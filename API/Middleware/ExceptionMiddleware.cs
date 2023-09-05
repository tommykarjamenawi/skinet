using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // If there is no exception, the request will continue on to the next piece of middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); // Logging the exception
                context.Response.ContentType = "application/json"; // Setting the content type of the response
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Setting the status code of the response

                var response = _env.IsDevelopment() // If the environment is development, return the exception message and stack trace
                    ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException((int)HttpStatusCode.InternalServerError); // If the environment is not development, return a generic message

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase}; // Creating options for the JsonSerializer
                var json = JsonSerializer.Serialize(response, options); // Serializing the response
                await context.Response.WriteAsync(json); // Writing the response to the HTTP response
            }
        }
    }
}