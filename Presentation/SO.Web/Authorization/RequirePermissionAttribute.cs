using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SO.Web.Authorization
{
    public class RequirePermissionAttribute : ActionFilterAttribute
    {
        private readonly string _permission;
        public RequirePermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var auth = context.HttpContext.RequestServices.GetService(typeof(IAppAuthorizationService)) as IAppAuthorizationService;
            var user = context.HttpContext.User;
            if (auth == null || !auth.IsAllowed(user, _permission))
            {
                context.Result = new ForbidResult();
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}


