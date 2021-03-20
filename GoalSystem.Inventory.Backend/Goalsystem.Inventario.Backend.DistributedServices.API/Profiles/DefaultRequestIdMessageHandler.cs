using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.API.Profiles
{
    public class DefaultRequestIdMessageHandler : DelegatingHandler
    {
        private readonly ICorrelationIdAccesor _correlationIdAccesor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultRequestIdMessageHandler(ICorrelationIdAccesor correlationIdAccesor, IHttpContextAccessor httpContextAccessor)
        {
            _correlationIdAccesor = correlationIdAccesor;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-Correlation-ID", _httpContextAccessor.HttpContext.TraceIdentifier);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
