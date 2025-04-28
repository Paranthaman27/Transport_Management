using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Transport_Management.Interface;
using Transport_Management.Models.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Transport_Management.Helpers.Filters
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var role = session.GetString("Role");
            var rurl = context.HttpContext.Request.Path;
            if (role == null || !_roles.Contains(role))
            {
                //var returnUrl = rurl; //context.HttpContext.Request.Path;
                //var accessDeniedUrl = $"{returnUrl}&accessDenied=true";

                context.Result = new RedirectToActionResult("Index", "Home", null);
                //context.HttpContext.Items["AccessDenied"] = true;
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
    }
}
