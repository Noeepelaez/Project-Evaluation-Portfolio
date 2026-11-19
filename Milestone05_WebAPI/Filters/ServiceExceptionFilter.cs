using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Milestone05_WebAPI.Services;

namespace Milestone05_WebAPI.Filters;

public class ServiceExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validation)
        {
            context.Result = Problem(StatusCodes.Status400BadRequest, "Validation error", validation.Message);
            context.ExceptionHandled = true;
        }
        else if (context.Exception is ConflictException conflict)
        {
            context.Result = Problem(StatusCodes.Status409Conflict, "Conflict", conflict.Message);
            context.ExceptionHandled = true;
        }
    }

    private static ObjectResult Problem(int status, string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };
        return new ObjectResult(problem) { StatusCode = status };
    }
}
