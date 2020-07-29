namespace Zrs
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NBitcoin;
    using NBitcoin.RPC;

    sealed class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Application Services.
            services.AddHostedService<DatabaseMigrator>();

            // Zsharp Services.
            services.AddServiceExceptionHandler();
            services.AddZcoin(Enum.Parse<NetworkType>(this.Configuration["Zcoin:Network"]));
            services.AddElysiumSerializer();
            services.AddZcoinRpcClient(options =>
            {
                var username = this.Configuration["Zcoin:Daemon:Rpc:Username"];
                var password = this.Configuration["Zcoin:Daemon:Rpc:Password"];

                options.ServerUrl = new Uri(this.Configuration["Zcoin:Daemon:Rpc:Address"]);
                options.Credential = RPCCredentialString.Parse($"{username}:{password}");
            });
            services.AddLightweightIndexer(options =>
            {
                options.BlockPublisherAddress = this.Configuration["Zcoin:Daemon:ZeroMq:Address"];
            });
            services.AddLightweightIndexerEntityRepository();
            services.AddLightweightIndexerPostgresDbContext(options =>
            {
                options.ConnectionString = this.Configuration["Database:Blockchain:ConnectionString"];
            });

            // ASP.NET Core Services.
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Build request processing pipeline so the order is very importance here.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceExceptionHandler("/service-error");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
