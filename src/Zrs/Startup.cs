namespace Zrs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Text.Json.Serialization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using NBitcoin;
    using NBitcoin.RPC;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Zrs.Options;
    using Zrs.Swagger;

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

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/{documentName}.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "Zcoin REST Service";
                options.RoutePrefix = string.Empty;
                options.SwaggerEndpoint("/openapi.json", "Zcoin REST Service");
            });
            app.UseServiceExceptionHandler("/service-error");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Application services.
            this.RegisterRuntimeConverters();
            this.ConfigureApiOptions(services, this.Configuration.GetSection("Api"));

            services.AddHostedService<DatabaseMigrator>();

            // Zsharp services.
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

            // ASP.NET Core services.
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("uint256", typeof(Constraints.UInt256));
            });

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    this.RegisterJsonConverters(options.JsonSerializerOptions.Converters);
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressMapClientErrors = true;
                });

            // Swashbuckle services.
            services.AddSwaggerGen(this.ConfigureSwaggerGenerator);
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

        void ConfigureSwaggerGenerator(SwaggerGenOptions options)
        {
            var document = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            options.SwaggerDoc("openapi", new OpenApiInfo()
            {
                Title = "Zcoin REST Service",
                Version = "1.0.0",
            });

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, document));

            options.MapType<uint256>(() => new OpenApiSchema() { Type = "string" });
            options.MapType<BitcoinAddress>(() => new OpenApiSchema() { Type = "string" });

            options.SchemaFilter<SchemaFilter>();
        }
    }
}
