using AccountingSystemService.Helpers;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountingSystemService.Validators
{
    public class CustomTokenValidator
    {
        private readonly string _jwtKey;

        public CustomTokenValidator(string jwtKey)
        {
            _jwtKey = jwtKey; // Секретный ключ для подписи токена
        }
        /// <summary>
        /// Метод для валидации токена авторизации
        /// </summary>
        /// <param name="token">Токен авторизации</param>
        /// <returns>Возвращает данные из токена если он валиден</returns>
        /// <exception cref="SecurityTokenException"> Если токен не валиден метод кидает данное исключение</exception>
        public ClaimsPrincipal ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new SecurityTokenException("Токен отсутствует.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            try
            {
                // Проверяем токен на валидность
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // Отключаем проверку издателя (если не используется)
                    ValidateAudience = false, // Отключаем проверку аудитории (если не используется)
                }, out SecurityToken validatedToken);


                if (!AuthServiceHelper.IsValid(token))
                {
                    throw new SecurityTokenException("Токен не валиден");
                }

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new SecurityTokenException("Токен истек.");
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException($"Ошибка валидации токена: {ex.Message}");
            }
        }
    }
}
