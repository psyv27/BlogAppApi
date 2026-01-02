using BlogApp.BL.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace BlogApp.API.Helpers
{
    public static class CustomExceptionHandler
    {
        public static void UseCustomExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(handlerApp =>
            {
                handlerApp.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    if (feature?.Error is IBaseException ex)
                    {
                        context.Response.StatusCode = ex.StatusCode;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            StatusCode = ex.StatusCode,
                            Message = ex.ErrorMessage
                        });
                    }
                    else if (feature?.Error is ArgumentNullException)
                    {
                        await context.Response.WriteAsJsonAsync(new
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = feature?.Error.Message
                        });
                    }
                });
            });
        }
    }
}
