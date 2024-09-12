using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace UserAPI.API
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Call the next middleware
            }
            catch (Exception ex)
            {
                var statusCode = HttpStatusCode.InternalServerError;
                var message = "An unexpected error occurred. Please try again later.";

                // Log the exception details
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);

                // Set response details
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var response = new { message };
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
