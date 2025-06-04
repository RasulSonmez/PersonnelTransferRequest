namespace PersonnelTransferRequest.Web.Middleware
{
    /// <summary>
    /// Custom middleware for logging HTTP requests in an ASP.NET Core application.
    /// Logs the request path, matched route pattern, authenticated username,
    /// and whether the request completed successfully or threw an exception.
    /// Designed to enhance auditability and traceability across both admin and personnel actions.
    /// </summary>

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Pre-request log
            var userName = context.User.Identity?.Name ?? "Anonymous";
            var requestPath = context.Request.Path;

            _logger.LogInformation("Request started: {Path} by user {User}", requestPath, userName);

            // To access MVC route data:
            // Endpoint metadata can be obtained with context.GetEndpoint()
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var routePattern = (endpoint as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern?.RawText;
                _logger.LogInformation("Matched route: {Route}", routePattern);
            }

            try
            {
                await _next(context); // Next middleware or action is run

                // Log after successful request
                _logger.LogInformation("Request finished successfully: {Path} by user {User}", requestPath, userName);
            }
            catch (Exception ex)
            {
                // Log if there is an error during the request
                _logger.LogError(ex, "Request failed: {Path} by user {User}", requestPath, userName);
                throw;
            }
        }
    }

}
