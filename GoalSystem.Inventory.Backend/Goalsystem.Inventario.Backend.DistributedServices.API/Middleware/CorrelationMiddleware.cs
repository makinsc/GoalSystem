using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.API.Middleware
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CorrelationIdOptions _options;

        public CorrelationMiddleware(RequestDelegate next, IOptions<CorrelationIdOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(_options.Header, out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
            }

            using (LogContext.PushProperty("CorrelationID", context.TraceIdentifier, true))
            {
                await this._next(context);
            }
        }
    }

    public static class CorrelationMiddlewareAppBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelationMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationMiddleware>();
        }
    }
}
