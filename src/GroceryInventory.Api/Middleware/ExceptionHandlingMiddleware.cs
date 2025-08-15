using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GroceryInventory.Api.Middleware;

public static class ExceptionHandling
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                ProblemDetails problem;
                int status;

                switch (exception)
                {
                    case ValidationException fv:
                        status = StatusCodes.Status400BadRequest;
                        var errors = fv.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                        problem = new ValidationProblemDetails(errors)
                        {
                            Title = "Validation failed",
                            Status = status,
                            Type = "https://httpstatuses.com/400"
                        };
                        break;
                    case KeyNotFoundException:
                        status = StatusCodes.Status404NotFound;
                        problem = new ProblemDetails
                        {
                            Title = "Not found",
                            Status = status,
                            Type = "https://httpstatuses.com/404"
                        };
                        break;
                    case UnauthorizedAccessException:
                        status = StatusCodes.Status401Unauthorized;
                        problem = new ProblemDetails
                        {
                            Title = "Unauthorized",
                            Status = status,
                            Type = "https://httpstatuses.com/401"
                        };
                        break;
                    case ArgumentException ae:
                        status = StatusCodes.Status400BadRequest;
                        problem = new ProblemDetails
                        {
                            Title = "Bad request",
                            Detail = ae.Message,
                            Status = status,
                            Type = "https://httpstatuses.com/400"
                        };
                        break;
                    default:
                        status = StatusCodes.Status500InternalServerError;
                        problem = new ProblemDetails
                        {
                            Title = "Server error",
                            Status = status,
                            Type = "https://httpstatuses.com/500"
                        };
                        break;
                }

                context.Response.StatusCode = status;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            });
        });
    }
}