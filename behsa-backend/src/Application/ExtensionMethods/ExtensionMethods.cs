using Microsoft.AspNetCore.Identity;

namespace Application.ExtensionMethods;

public static class ExtensionMethods
{
    public static string FirstMessage(this IEnumerable<IdentityError> errors)
    {
        return errors.FirstOrDefault()?.Description ?? "Unknown Error.";
    }
}