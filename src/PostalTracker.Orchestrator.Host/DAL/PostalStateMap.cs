using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostalTracker.Orchestrator.Host.Components;

namespace PostalTracker.Orchestrator.Host.DAL;

public class PostalStateMap : SagaClassMap<PostalState>
{
    protected override void Configure(EntityTypeBuilder<PostalState> entity, ModelBuilder model)
    {
        entity.ToTable("PostalStates");
        entity.HasKey(x => x.CorrelationId).HasName("PK_UserProfile");

        entity.Property(x => x.CorrelationId).HasColumnType("uuid").IsRequired();
        entity.Property(x => x.InWayDurationToken).HasColumnType("uuid").IsRequired(false);
        entity.Property(x => x.CurrentState).HasColumnType("varchar(50)").IsRequired();
        entity.Property(x => x.AddressDelivery).HasColumnType("varchar(250)").IsRequired().HasMaxLength(250);
        entity.Property(x => x.AddressSender).HasColumnType("varchar(250)").IsRequired().HasMaxLength(250);
        entity.Property(x => x.Timestamp).HasColumnType("varchar(250)").IsRequired().HasMaxLength(250);
    }
}