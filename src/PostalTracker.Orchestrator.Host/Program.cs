using System.Reflection;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using PostalTracker.Orchestrator.Host;
using PostalTracker.Orchestrator.Host.Components;
using PostalTracker.Orchestrator.Host.DAL;
using Quartz;
using Quartz.Spi;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        services.AddQuartz(x => x.UseMicrosoftDependencyInjectionJobFactory());
        services.AddMassTransit(mt =>
        {
            var quartzQueueName = builder.Configuration.GetValue<string>("QuartzQueue");
            mt.AddMessageScheduler(new Uri(quartzQueueName));

            mt.AddSagaStateMachine<PostalStateMachine, PostalState>().
                EntityFrameworkRepository(ef =>
                {
                    ef.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    ef.AddDbContext<DbContext, PostalStateContext>((provider, efBuilder) =>
                        {
                            efBuilder.UseNpgsql(builder.Configuration.GetConnectionString("PostalStateContext"), m =>
                            {
                                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                m.MigrationsHistoryTable($"__{nameof(PostalStateContext)}");
                            });
                        });
                    
                });
            
            
            mt.SetKebabCaseEndpointNameFormatter();
            mt.UsingRabbitMq((context, cfg) =>
            {
                cfg.AutoStart = true;
        
                var redditMqSection = builder.Configuration.GetSection("RabbitMq");
                cfg.Host(redditMqSection.GetValue<string>("HostAddress"),
                    redditMqSection.GetValue<ushort>("Port"),
                    redditMqSection.GetValue<string>("VirtualHost"), h =>
                    {
                        h.Username(redditMqSection.GetValue<string>("Username"));
                        h.Password(redditMqSection.GetValue<string>("Password"));
                    });
                
                cfg.UseInMemoryScheduler(schedulerCfg =>
                {
                    schedulerCfg.QueueName = quartzQueueName;
                    schedulerCfg.SchedulerFactory = context.GetRequiredService<ISchedulerFactory>();
                    schedulerCfg.JobFactory = context.GetRequiredService<IJobFactory>();
                    schedulerCfg.StartScheduler = false;
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        services.AddHostedService<MassTransitHostedService>();
        services.AddQuartzHostedService(
            q => q.WaitForJobsToComplete = true);
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<DbContext>();
    context?.Database.Migrate();
}

await host.RunAsync();