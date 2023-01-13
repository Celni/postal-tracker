using Automatonymous;
using MassTransit;
using PostalTracker.Contracts.Events;

namespace PostalTracker.Orchestrator.Host.Components;

public class PostalStateMachine : MassTransitStateMachine<PostalState>
{
    public PostalStateMachine()
    {
        Event(() => PostalCreate, x => x.CorrelateById(q => q.Message.Id));
        Event(() => PostalPaid, x => x.CorrelateById(q => q.Message.Id));
        Event(() => PostalReceived, x => x.CorrelateById(q => q.Message.Id));
        Event(() => PostalLost, x => x.CorrelateById(q => q.Message.Id));
        Event(() => PostalReturn, x => x.CorrelateById(q => q.Message.Id));
        Event(() => PostalStatusRequest, x =>
        {
            x.CorrelateById(m => m.Message.Id);
            x.OnMissingInstance(m => m.ExecuteAsync(async context =>
            {
                if (context.RequestId.HasValue)
                {
                    await context.RespondAsync<PostalNotFound>(new { context.Message.Id });
                }
            }));
        });

        Schedule(() => PostalInWayExpired, x => x.InWayDurationToken, cfg =>
        {
            cfg.Delay = TimeSpan.FromMinutes(1);
            cfg.Received = x => x.CorrelateById(q => q.Message.Id);
        });

        InstanceState(x => x.CurrentState);

        Initially(
            When(PostalCreate)
                .Then(context =>
                {
                    context.Instance.Timestamp = context.Data.Timestamp;
                    context.Instance.AddressDelivery = context.Data.AddressDelivery;
                    context.Instance.AddressSender = context.Data.AddressSender;
                })
                .TransitionTo(Created));

        During(Created,
            When(PostalPaid)
                .Schedule(PostalInWayExpired, context => context.Init<PostalDelivered>(new { context.Data.Id }))
                .TransitionTo(Paid)
                .TransitionTo(InWay));

        During(InWay,
            When(PostalLost)
                .Unschedule(PostalInWayExpired)
                .TransitionTo(Lost),
            When(PostalInWayExpired.Received)
                .TransitionTo(Delivered));

        During(Delivered,
            When(PostalReceived)
                .TransitionTo(Received)
                .TransitionTo(Archived),
            When(PostalReturn)
                .Then(context =>
                {
                    var addressDelivery = context.Instance.AddressDelivery;
                    var addressSender = context.Instance.AddressSender;
                    context.Instance.AddressDelivery = addressSender;
                    context.Instance.AddressSender = addressDelivery;
                    context.Instance.Timestamp = InVar.Timestamp;
                })
                .TransitionTo(Return)
                .Schedule(PostalInWayExpired, context => context.Init<PostalDelivered>(new { context.Data.Id }))
                .TransitionTo(InWay));

        DuringAny(
            When(PostalStatusRequest)
                .RespondAsync(x => x.Init<PostalStatus>(new
                {
                    Id = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState,
                    x.Instance.AddressDelivery,
                    x.Instance.AddressSender,
                    x.Instance.Timestamp
                })));
    }

    public State Created { get; set; }
    public State Paid { get; set; }
    public State InWay { get; set; }
    public State Delivered { get; set; }
    public State Received { get; set; }
    public State Archived { get; set; }
    public State Lost { get; set; }
    public State Return { get; set; }

    public Event<PostalCreate> PostalCreate { get; private set; }
    public Event<PostalPaid> PostalPaid { get; private set; }
    public Event<PostalReceived> PostalReceived { get; private set; }
    public Event<PostalLost> PostalLost { get; private set; }
    public Event<PostalReturn> PostalReturn { get; private set; }
    public Event<PostalStatusRequest> PostalStatusRequest { get; private set; }

    public Schedule<PostalState, PostalDelivered> PostalInWayExpired { get; private set; }
}