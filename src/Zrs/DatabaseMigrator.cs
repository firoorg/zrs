namespace Zrs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Zsharp.Entity;

    sealed class DatabaseMigrator : IHostedService
    {
        readonly ILogger logger;
        readonly IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext> chain;

        public DatabaseMigrator(
            ILogger<DatabaseMigrator> logger,
            IDbContextFactory<Zsharp.LightweightIndexer.Entity.DbContext> chain)
        {
            this.logger = logger;
            this.chain = chain;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Blockchain.
            this.logger.LogInformation("Migrating blockchain database.");

            await using (var db = await this.chain.CreateAsync(cancellationToken))
            {
                await db.Database.MigrateAsync(cancellationToken);
            }

            this.logger.LogInformation("All database migrations completed successfully.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
