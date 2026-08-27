using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WarehouseSystem.Helpers;

namespace WarehouseSystem.Filters;

public class RequireAuthAttribute : Attribute, IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.HttpContext.Session.GetInt32(SessionKeys.UserId);

        if (userId == null)
        {
            context.Result = new UnauthorizedResult(); // chyba 401
            return Task.CompletedTask;
        }

        return next();
    }
}