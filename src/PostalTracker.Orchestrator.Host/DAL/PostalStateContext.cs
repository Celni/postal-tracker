using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace PostalTracker.Orchestrator.Host.DAL;

public class PostalStateContext : SagaDbContext
{
    public PostalStateContext(DbContextOptions options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations 
        => new[] { new PostalStateMap() };
}