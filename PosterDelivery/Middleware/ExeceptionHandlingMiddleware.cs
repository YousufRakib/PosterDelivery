using PosterDelivery.Utility;
using Microsoft.AspNetCore.Mvc;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;
using System.Net;
using System.Security.Claims;

namespace PosterDelivery.Middleware {
    public class ExeceptionHandlingMiddleware {

        public RequestDelegate _requestDelegate;
        public ExeceptionHandlingMiddleware(RequestDelegate requestDelegate) {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context, ILoggerService loggerService) {
            try {
                await _requestDelegate(context);
            } catch (Exception ex) {
                Error error = GetError(context,ex);
                // Creating logs to DB Table - Exception Logs
                if (error != null) {
                    loggerService.LogExceptionInformation(error);
                }
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.WriteAsJsonAsync(new Confirmation() { msg = "Something went wrong!!", output = "Error" });
            }
        }

        private Error GetError(HttpContext context,Exception ex) {
            Error error = null;
            if (ex != null) {
                error = new Error();
                error.Message = ex.Message;
                error.Source = context.Request.Path;
                error.StackTrace = ex.StackTrace;
                error.UserId =context.User.Claims?.FirstOrDefault(x=>x.Type == ClaimTypes.NameIdentifier)?.Value;
                error.Type = ex.GetType().Name;
                error.StatusCode = HttpStatusCode.InternalServerError.ToString();
            }
            return error;
        }




    }
}
