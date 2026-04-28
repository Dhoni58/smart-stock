using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WarehouseSystem.Helpers;

namespace WarehouseSystem.Filters;

public class AuthFilter : IPageFilter
{
    public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
    public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        var path = context.HttpContext.Request.Path.Value ?? "";

        if (path.StartsWith("/Login", StringComparison.OrdinalIgnoreCase))
            return;

        if (path.StartsWith("/Logout", StringComparison.OrdinalIgnoreCase))
            return;

        var userId = context.HttpContext.Session.GetInt32(SessionKeys.UserId);

        if (userId == null)
        {
            context.Result = new RedirectToPageResult("/Login");
            return;
    }

        // Stránky pouze pro vedoucího
        if (path.StartsWith("/Users", StringComparison.OrdinalIgnoreCase))
        {
            var role = context.HttpContext.Session.GetString(SessionKeys.UserRole);
            if (role != "Vedouci")
                context.Result = new RedirectToPageResult("/Index");
        }
    }

    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var path = context.HttpContext.Request.Path.Value ?? "";

        // Login stránka nevyžaduje přihlášení
        if (path.StartsWith("/Login", StringComparison.OrdinalIgnoreCase))
            return;

        var userId = context.HttpContext.Session.GetInt32(SessionKeys.UserId);

        if (userId == null)
            context.Result = new RedirectToPageResult("/Login");
    }
    
}