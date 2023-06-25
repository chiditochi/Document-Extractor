using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Document_Extractor.Configuration.Filters;
public class AppAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userName = context.HttpContext.Session.GetString("username");
        if(userName is null)
        {
            context.Result = new RedirectResult("/Login");
        }
        else
        {
            //context.Result = new RedirectResult("/Upload");
        }

    }
}
