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
                GotrueException => HandleGotrueException2(exception),
                ArgumentNullException => (HttpStatusCode.BadRequest, "A required value was not provided", exception.Message),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid input provided", exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", exception.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found", exception.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation", exception.Message),
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


            // // Include inner exception messages
            // var errors = new List<string>();
            // var currentException = exception;
            // while (currentException != null)
            // {
            //     errors.Add(currentException.Message);
            //     currentException = currentException.InnerException;
            // }
            // problemDetails.Extensions["errors"] = errors;

            // context.Response.StatusCode = problemDetails.Status.Value;
            // context.Response.ContentType = "application/problem+json";

            // await context.Response.WriteAsJsonAsync(problemDetails);
        }

        private record SupabaseErrorResponse(int code, string? error_code, string? msg);
        private (HttpStatusCode status, string title, string detail) HandleGotrueException2(Exception exception)
        {
            return JsonSerializer.Deserialize<SupabaseErrorResponse>(exception.Message) switch
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
    }
}



// implementation without ProblemDetails
// using System.Net;
// using System.Text.Json;

// namespace HobbyCom.Presenter.API.src.Middlewares
// {
//     public class GlobalExceptionMiddleware : IMiddleware
//     {
//         private readonly ILogger<GlobalExceptionMiddleware> _logger;
//         private readonly IHostEnvironment _env;

//         public GlobalExceptionMiddleware(
//             ILogger<GlobalExceptionMiddleware> logger,
//             IHostEnvironment env)
//         {
//             _logger = logger;
//             _env = env;
//         }

//         public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//         {
//             try
//             {
//                 await next(context);
//             }
//             catch (Exception ex)
//             {
//                 await HandleExceptionAsync(context, ex);
//             }
//         }

//         private async Task HandleExceptionAsync(HttpContext context, Exception exception)
//         {
//             _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

//             if (context.Response.HasStarted)
//             {
//                 _logger.LogWarning("Response has already started, unable to modify headers");
//                 return;
//             }

//             var (status, message) = exception switch
//             {
//                 Supabase.Gotrue.Exceptions.GotrueException => (HttpStatusCode.BadRequest, "Authentication failed"),
//                 ArgumentNullException => (HttpStatusCode.BadRequest, "A required value was not provided"),
//                 ArgumentException => (HttpStatusCode.BadRequest, "Invalid input provided"),
//                 UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "You are not authorized to perform this action"),
//                 KeyNotFoundException => (HttpStatusCode.NotFound, "The requested resource was not found"),
//                 InvalidOperationException => (HttpStatusCode.BadRequest, "The requested operation is invalid"),
//                 _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
//             };

//             var errors = new List<string>();

//             if (!string.IsNullOrEmpty(exception.Message))
//             {
//                 errors.Add(exception.Message);
//             }

//             var currentException = exception.InnerException;
//             while (currentException != null)
//             {
//                 if (!string.IsNullOrEmpty(currentException.Message))
//                 {
//                     errors.Add(currentException.Message);
//                 }
//                 currentException = currentException.InnerException;
//             }

//             var response = new
//             {
//                 Success = false,
//                 StatusCode = (int)status,
//                 Message = message,
//                 Errors = errors,
//                 StackTrace = _env.IsDevelopment() ? exception.StackTrace : null,
//                 Timestamp = DateTime.UtcNow,
//                 TraceId = context.TraceIdentifier
//             };

//             context.Response.StatusCode = (int)status;
//             context.Response.ContentType = "application/json";

//             var options = new JsonSerializerOptions
//             {
//                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//                 WriteIndented = _env.IsDevelopment()
//             };

//             await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
//         }
//     }
// }