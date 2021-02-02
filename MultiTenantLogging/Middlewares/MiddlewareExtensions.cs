using Microsoft.AspNetCore.Builder;
using MultiTenantLogging.Middlewwares;

namespace MultiTenantLogging
{
    /// <summary>
    /// Middleware Extensions
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// UseMultiTenantLogging
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMultiTenantLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MultiTenantLoggingMiddleware>();
        }
    }
}
