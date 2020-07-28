namespace Zrs
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    static class Program
    {
        static void Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                });

            hostBuilder.Build().Run();
        }
    }
}
