using System.Net;
using System.Text.Json;
using SharedKernel.Exceptions;

namespace BookStore.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошло необработанное исключение: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        // Определяем статус-код на основе типа исключения из SharedKernel
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            // Сюда можно добавить ValidationException => BadRequest и т.д.
            BaseException => HttpStatusCode.BadRequest, // Все наши кастомные по умолчанию 400
            _ => HttpStatusCode.InternalServerError   // Неизвестная ошибка (баг) — 500
        };

        context.Response.StatusCode = (int)statusCode;

        // Формируем красивый ответ
        var response = new
        {
            error = exception.Message,
            errorCode = (exception as BaseException)?.ErrorCode ?? "internal_server_error",
            // В продакшене StackTrace лучше не выводить, только в режиме Development
            // detail = exception.StackTrace 
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}