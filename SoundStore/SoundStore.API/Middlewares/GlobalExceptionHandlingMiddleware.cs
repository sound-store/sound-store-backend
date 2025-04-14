using SoundStore.Core.Commons;
using SoundStore.Core.Exceptions;
using System.Net;

namespace SoundStore.API.Middlewares
{
    public class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger) : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}", ex.StackTrace);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the exception and writes an error response.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="ex">The exception that occurred.</param>
        /// <returns>A task that represents the completion of writing the response.</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var statusCode = HttpStatusCode.InternalServerError;
            switch (ex)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case NoDataRetrievalException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case NotSupportedException:
                    statusCode = HttpStatusCode.NotImplemented;
                    break;
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
                case ForbiddenException:
                    statusCode = HttpStatusCode.Forbidden;
                    break;
                case DuplicatedException:
                    statusCode = HttpStatusCode.Conflict;
                    break;
                default:
                    break;
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var result = new ErrorResponse
            {
                Message = ex.Message,
                Details = env == "Development" ? ex.StackTrace : null,
            };

            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
