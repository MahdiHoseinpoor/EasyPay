using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace EasyPay.Api.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IEndpointConventionBuilder MapCustomHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = WriteHealthCheckResponse
            });
        }

        private static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration
                }),
                totalDuration = report.TotalDuration
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}