using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace GoalSystem.Inventario.Backend.API.Profiles
{
    public class CorrelationIdAccesor : ICorrelationIdAccesor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CorrelationIdAccesor> _logger;

        public CorrelationIdAccesor(ILogger<CorrelationIdAccesor> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._httpContextAccessor = httpContextAccessor;
        }

        public string GetCorrelationId()
        {
            try
            {
                var context = this._httpContextAccessor.HttpContext;
                var result = context?.Items["X-CorrelationId"] as string;

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Unable to get original session id header");
            }

            return string.Empty;
        }
    }
}
