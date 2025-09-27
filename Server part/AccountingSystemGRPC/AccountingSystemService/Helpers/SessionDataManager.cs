using Microsoft.Extensions.Hosting;

using System.Collections.Concurrent;

namespace AccountingSystemService.Helpers
{
    public static class SessionDataManager
    {
        private static ConcurrentDictionary<string, SessionData> SessionDataForUser { get; } = [];

        public static SessionData? GetUserData(string id)
        {
            try
            {
                if (SessionDataForUser.TryGetValue(id, out SessionData? sessionData))
                {
                    return sessionData;
                }
            }
            catch
            {

            }
            return null;
        }

        public static bool TryAddUser(string id)
        {
            try
            {
                return SessionDataForUser.TryAdd(id, new SessionData());
            }
            catch
            {

            }
            return false;
        }

        public static bool TryRemoveUser(string id)
        {
            try
            {
                if (SessionDataForUser.TryGetValue(id, out SessionData? sessionData))
                {
                    var keyValuePair = new KeyValuePair<string, SessionData>(id, sessionData);
                    return SessionDataForUser.TryRemove(keyValuePair);
                }
            }
            catch
            {

            }
            return false;
        }
    }

    public class SessionData
    {
        public string ErrorMessage { get; set; } = "";

        public bool IsBackupDone { get; set; } = false;

        public string BackupFilePath { get; set; } = "";
    }
}
