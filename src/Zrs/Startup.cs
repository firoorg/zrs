namespace Zrs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.Json.Serialization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using NBitcoin;
    using NBitcoin.RPC;
    using Zrs.Options;

    sealed class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

        public void ConfigureServices(IServiceCollection services)
        {
            // Application Services.
            this.RegisterRuntimeConverters();
            this.ConfigureApiOptions(services, this.Configuration.GetSection("Api"));

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
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("uint256", typeof(Constraints.UInt256));
            });

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    this.RegisterJsonConverters(options.JsonSerializerOptions.Converters);
                });
        }

        void ConfigureApiOptions(IServiceCollection services, IConfigurationSection section)
        {
            services
                .AddOptions<BlocksApiOptions>()
                .Bind(section.GetSection("Blocks"))
                .ValidateDataAnnotations();
        }

        void RegisterRuntimeConverters()
        {
            Register(typeof(uint256), typeof(Converters.Runtime.UInt256));

            static void Register(Type type, Type converter)
            {
                TypeDescriptor.AddAttributes(type, new TypeConverterAttribute(converter));
            }
        }

        void RegisterJsonConverters(IList<JsonConverter> converters)
        {
            Register(new Converters.Json.UInt256());

            void Register(JsonConverter converter)
            {
                converters.Add(converter);
            }
        }
    }
}
