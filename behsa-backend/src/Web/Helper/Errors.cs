using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Helper;

public static class Errors
{
    public static ErrorResponse New(string title, string message)
    {
        return new ErrorResponse
        {
            Title = title,
            Message = new List<string> { message }
        };
    }
}