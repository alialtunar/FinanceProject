using FinanceProject.Core.Exceptions;

// ExceptionHandlingMiddleware.cs

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
namespace FinanceProject.WebApi.Middleware
{
    

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ErrorException errorException)
            {
                await HandleExceptionAsync(context, errorException);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, new ErrorException(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, ErrorException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception.StatusCode;

            var response = new
            {
                error = "Error",
                message = exception.Message
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }

}
