using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToDo.Application.Exceptions;
using System.Net;

namespace ToDo.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path //Hatanın hangi api adresinde olduğunu yakalar. 
            };

            if (exception is BaseException baseException)
            {
                problemDetails.Status = baseException.StatusCode;
                problemDetails.Title = baseException.Title;
                problemDetails.Detail = exception.Message;

                // BaseException'dan gelen hataları yakalama
                //_logger.LogWarning("business rule violation: {Message}", exception.Message);
            }

            //.net'in kendi hataları için else if eklemek lazım
            else
            {
                //Hata beklenmeyen bir şey ise
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "System Server Error";
                problemDetails.Detail = "An unexpected situation occurred while the transaction was being processed.";

                _logger.LogError(exception, "Error Code 500 in {path}", httpContext.Request.Path);
            }

            httpContext.Response.StatusCode = (int)problemDetails.Status;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            
            return true;
        }
    }
}