using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;

namespace AccountingSystemService.Helpers
{
    public static class AuthServiceHelper
    {
        public const string JwtKey = "KEYCENSORISTHEBESTKEYCENSORISTHEBESTdKEYCENSORISTHEBEST";

        /// <summary>
        /// Хранилище токенов и времени их создания
        /// </summary>
        public static ConcurrentDictionary<string, DateTime> Tokens { get; } = new();
        private static TimeSpan maxTimeOfInaction;
        public static int CountOfClients = 0;
        private static Timer timerToDeleteUnusedTokens;
        private static int periodBetweenCallbacks = 60000;

        static AuthServiceHelper()
        {
            string json = File.ReadAllText("appsettings.json");
            using JsonDocument doc = JsonDocument.Parse(json);
            maxTimeOfInaction = TimeSpan.FromMinutes(double.Parse(doc.RootElement.GetProperty("MaxTimeOfInaction").GetString() ?? "20"));
            timerToDeleteUnusedTokens = new(new TimerCallback(x =>
            {
                foreach (var token in Tokens.Keys.ToList())
                {
                    if (DateTime.UtcNow - Tokens[token] > maxTimeOfInaction)
                    {
                        Tokens.Remove(token, out DateTime value);
                    }
                }
            }), null, 0, periodBetweenCallbacks);
        }
        /// <summary>
        /// Метод для обновления времени последней активности
        /// </summary>
        /// <param name="token">Токен авторизации</param>
        public static void UpdateLastActivity(string token)
        {
            Tokens[token] = DateTime.UtcNow;
        }
        /// <summary>
        /// Метод для проверки токена на валидность, т.е. выдан ли он службой и проверка на время бездействия
        /// </summary>
        /// <param name="token">Токен авторизации</param>
        /// <returns>true  - если токен валиден, false - если не валиден </returns>
        public static bool IsValid(string token)
        {
            if (Tokens.Keys.FirstOrDefault(x => x == token) is null)
            {
                return false;
            }
            //if (DateTime.UtcNow - Tokens[token] > maxTimeOfInaction)
            //{
            //    Tokens.Remove(token, out DateTime value);
            //    return false;
            //}
            return true;
        }
    }
}
