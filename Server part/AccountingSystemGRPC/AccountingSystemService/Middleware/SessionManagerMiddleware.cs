using AccountingSystemService.Helpers;
using AccountingSystemService.Validators;

using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using Microsoft.IdentityModel.Tokens;

namespace AccountingSystemService.Middleware
{
    public class SessionManagerMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionManagerMiddleware(RequestDelegate next)
        {
            _next = next;
            
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var id = context.Request.Headers["Id"].ToString();

            if (!string.IsNullOrEmpty(id))
            {
                SessionDataManager.TryAddUser(id);
            }

            // Передаем управление следующему middleware
            await _next(context);
            return;
        }
    }
}
