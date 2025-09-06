using AccountingSystemService.Helpers;
using AccountingSystemService.Validators;

using Microsoft.IdentityModel.Tokens;

namespace AccountingSystemService.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CustomTokenValidator _tokenValidator;

        public TokenValidationMiddleware(RequestDelegate next, CustomTokenValidator tokenValidator)
        {
            _next = next;
            _tokenValidator = tokenValidator;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var excludedPaths = new List<string>
            {
                "/Authentification.AuthentificationService/Authentificate",// Маршрут аутентификации,
                "/Authentification.AuthentificationService/CallBack",
                "/v1/auth"// Маршрут аутентификации
            };
            if (context.Request.Path.HasValue)
            {
                bool result = false;
                foreach (var path in excludedPaths)
                {
                    result = context.Request.Path.Value.Contains(path);
                    if (result)
                    {
                        break;
                    }
                }
                if (result)
                {
                    await _next(context);
                    return;
                }
            }
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Валидируем токен
                    var principal = _tokenValidator.ValidateToken(token);

                    // Устанавливаем пользовательские данные в контекст
                    context.User = principal;
                    //Обновляем время последней активности
                    AuthServiceHelper.UpdateLastActivity(token);
                }
                catch (SecurityTokenException ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Access denied");
                return;
            }

            // Передаем управление следующему middleware
            await _next(context);
            return;
        }
    }
}
