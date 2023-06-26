using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Document_Extractor.Configuration.Filters;
public class AppUserTypeFilterAttribute : Attribute, IActionFilter
{
    private readonly string _userType;
    public AppUserTypeFilterAttribute(string userType)
    {
        _userType = userType;
    }
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var currentUserType = context.HttpContext.Session.GetString("usertype");
        if (currentUserType is null)
        {
            context.Result = new RedirectResult("/Login");
            return;
        }
        if (currentUserType != _userType)
        {
            if (currentUserType != "OpTeam")
            {
                context.Result = new RedirectResult("/");
                return;
            }
            else
            {
                context.Result = new RedirectResult("/Upload");
                return;

            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }

}
