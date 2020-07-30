namespace Zrs.Controllers
{
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Hosting;
    using Zrs.Models;
    using Zsharp.ServiceModel.AspNetCore;

    [ApiController]
    public sealed class ServiceErrorController : ControllerBase
    {
        readonly IHostEnvironment environment;

        public ServiceErrorController(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        [Route("service-error")]
        public IActionResult ServiceError()
        {
            var feature = HttpContext.Features.Get<ServiceExceptionFeature>();
            var details = new ProblemDetails()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "The required background services was stopped unexpectedly."
            };

            if (feature != null && this.environment.IsDevelopment())
            {
                var errors = feature.Exceptions.Select(
                    e => new ServiceError()
                    {
                        Service = e.Service.FullName,
                        Error = e.Exception.Message,
                        Details = e.Exception.StackTrace,
                    });

                details.Extensions.Add("errors", errors);
            }

            return StatusCode(details.Status.Value, details);
        }
    }
}
