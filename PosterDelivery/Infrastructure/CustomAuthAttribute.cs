using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NuGet.Configuration;
using System;

namespace PosterDelivery.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthAttribute : Attribute, IAuthorizationFilter
    {
       public string? Roles { get; set; }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Session.GetString("Roles") == null)
            {
                context.HttpContext.Response.Redirect("/User/Login");
            }
            else
            {
                var userRole = context.HttpContext.Session.GetString("Roles");
                string[] roles = Roles.Split(new char[] { ',' });
                if (!roles.Any(r => userRole.Contains(r)))
                {
                    context.HttpContext.Response.Redirect("/Account/AccessDenied");
                }
            }
        }
    }
}
