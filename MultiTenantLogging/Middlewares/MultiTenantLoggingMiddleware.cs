using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiTenantLogging.Middlewwares
{
    /// <summary>
    /// MultiTenantLoggingMiddleware
    /// </summary>
    public class MultiTenantLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public MultiTenantLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var tenantIdentifier = httpContext?.GetMultiTenantContext<TenantInfo>()?.TenantInfo?.Identifier;
            tenantIdentifier ??= "Tenant: None";

            LogContext.PushProperty("TenantIdentifier", $"Tenenat: {tenantIdentifier}");
            await _next.Invoke(httpContext);
        }
    }
}
