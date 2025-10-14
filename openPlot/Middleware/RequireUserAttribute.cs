using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using openPlot.Web.Session;

namespace openPlot.Middleware;

/// <summary>Use em actions que exigem usuário na sessão, antes de termos Keycloak real.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireUserAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var session = context.HttpContext.RequestServices.GetRequiredService<ISessionUserService>();
        var user = session.GetCurrentUser();
        if (user is null)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Sessão expirada ou inexistente." });
            return;
        }
        await next();
    }
}
