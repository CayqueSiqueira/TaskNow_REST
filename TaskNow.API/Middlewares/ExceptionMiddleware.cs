using System.Net;
using System.Text.Json;
using TaskNow.DTO.Utils;

namespace TaskNow.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro nao tratado");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = RetornoDTO<object>.Fail("Erro inesperado. Tente novamente.");
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}