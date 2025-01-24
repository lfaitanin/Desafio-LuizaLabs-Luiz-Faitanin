using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Extensions;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails CreateProblemDetails(HttpContext context, string title, string detail, int statusCode)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context?.Request.Path
        };
    }
}
