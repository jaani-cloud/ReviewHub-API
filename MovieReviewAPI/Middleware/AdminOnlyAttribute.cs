using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MovieReviewAPI.Middleware;

public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        //Console.WriteLine($"IsAuthenticated: {user.Identity?.IsAuthenticated}");
        //Console.WriteLine($"Claims count: {user.Claims.Count()}");

        foreach (var claim in user.Claims)
        {
            //Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
        }

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                success = false,
                message = "Unauthorized. Please login."
            });
            return;
        }

        var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
        //Console.WriteLine($"Role claim: {roleClaim}");

        if (string.IsNullOrEmpty(roleClaim) || roleClaim != "Admin")
        {
            context.Result = new ObjectResult(new
            {
                success = false,
                message = "Access denied. Admin only."
            });
            return;
        }
    }
} 