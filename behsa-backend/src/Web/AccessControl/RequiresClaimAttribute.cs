using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.AccessControl;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresAnyRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _claimName;
    private readonly string[] _roles;

    public RequiresAnyRoleAttribute(string claimName, params string[] roles)
    {
        _claimName = claimName;
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        if (user.Identity is { IsAuthenticated: false })
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var hasRequiredRole = _roles.Any(role => user.HasClaim(_claimName, role));

        if (!hasRequiredRole)
        {
            context.Result = new ForbidResult();
        }
    }
}