using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Api.Framework
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var httpResponse = HttpStatusCode.InternalServerError;
            var errorCode = "InternalError";

            if (ex is ApplicationException appEx)
            {
                httpResponse = HttpStatusCode.BadRequest;
                errorCode = appEx.Code;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpResponse;
            return context.Response.WriteAsync(errorCode);
        }
    }
}