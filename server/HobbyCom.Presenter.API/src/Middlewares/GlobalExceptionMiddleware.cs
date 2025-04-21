using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Supabase.Gotrue.Exceptions;

namespace HobbyCom.Presenter.API.src.Middlewares
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, unable to modify headers");
                return;
            }

            var (status, title, detail) = exception switch
            {
                GotrueException => HandleGotrueException(exception),
                ArgumentNullException => (HttpStatusCode.BadRequest, "A required value was not provided", exception.Message),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid input provided", exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", exception.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found", exception.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation", exception.Message),
                Exception ex when ContainsJson(ex.Message) => HandleGotrueException(ex),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred", exception.Message)
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)status,
                Title = title,
                Detail = detail,
                Type = $"https://httpstatuses.com/{(int)status}",
                Instance = context.Request.Path
            };

            // Add extensions
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

            if (_env.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            // Set errors to only contain the detail message. This is useful for client-side error handling.
            problemDetails.Extensions["errors"] = new[] { detail };

            context.Response.StatusCode = problemDetails.Status.Value;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);

        }

        private record SupabaseErrorResponse(int code, string? error_code, string? msg);

        private (HttpStatusCode status, string title, string detail) HandleGotrueException(Exception exception)
        {
            // Extract JSON substring from the exception message
            var jsonStart = exception.Message.IndexOf('{');
            var jsonEnd = exception.Message.LastIndexOf('}');

            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonSubstring = exception.Message.Substring(jsonStart, jsonEnd - jsonStart + 1);

                return JsonSerializer.Deserialize<SupabaseErrorResponse>(jsonSubstring) switch
                {
                    { msg: not null } error => (
                        HttpStatusCode.BadRequest,
                        "Authentication failed",
                        error.msg),

                    _ => (
                        HttpStatusCode.BadRequest,
                        "Authentication failed",
                        exception.Message)
                };
            }

            // If no JSON is found, return the original message
            return (
                HttpStatusCode.BadRequest,
                "Authentication failed",
                exception.Message
            );
        }

        private static bool ContainsJson(string message)
        {
            return message.IndexOf('{') >= 0 && message.LastIndexOf('}') > message.IndexOf('{');
        }
    }
}