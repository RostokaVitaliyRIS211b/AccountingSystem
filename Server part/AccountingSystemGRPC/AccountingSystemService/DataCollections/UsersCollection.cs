using AccountingSystemService.Helpers;
using AccountingSystemService.Interfaces;
using AccountingSystemService.Wrappers;

using BdClasses;

using System.Xml.Linq;

namespace AccountingSystemService.DataCollections
{
    public class UsersCollection
    {
        public UsersCollection(IErrorHandler errorHandler) 
        {
            ErrorHandler = errorHandler;

            try
            {
                using var db = DbContextHelper.GetConstructionContext();
                var users = db.Users.ToList();
                var rolesForUsers = db.RolesOfUsers.ToList().ToLookup(x=>x.UserId);
                var roles = db.Roles.ToList();
                var permForRoles = db.PermissionsForRoles.ToList().ToLookup(x=>x.RoleId);
                var permissions = db.Permissions.ToList();

                foreach(var user in users)
                {
                    var uWrapper = new UserWrapper();
                    uWrapper.Id = user.Id;
                    uWrapper.Name = user.Name;
                    uWrapper.Description = user.Description ?? "";
                    uWrapper.Roles = [];
                    foreach(var role in rolesForUsers[user.Id].Select(x=>x.RoleId))
                    {
                        uWrapper.Roles.Add(role);
                    }
                    Users.Add(uWrapper);
                }

                errorHandler.HandleError($"Загрузка пользователей завершена",Severity.Information);

                foreach(var role in roles)
                {
                    var rWrapper = new RoleWrapper();
                    rWrapper.Id = role.Id;
                    rWrapper.Name = role.Name;
                    rWrapper.Description = role.Description ?? "";
                    rWrapper.Permissions = [];
                    foreach(var perm in permForRoles[role.Id].Select(x=>x.PermId))
                    {
                        rWrapper.Permissions.Add(perm);
                    }
                    Roles.Add(rWrapper);
                }

                errorHandler.HandleError($"Загрузка ролей завершена", Severity.Information);

                foreach(var perm in permissions)
                {
                    var pWrapper = new PermissionWrapper();
                    pWrapper.Id = perm.Id;
                    pWrapper.Name = perm.Name;
                    Permissions.Add(pWrapper);
                }

                errorHandler.HandleError($"Загрузка разрешений завершена", Severity.Information);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при загрузке инфориации по пользователям -> {e.Message}",Severity.Error);
            }

        }

        private IErrorHandler ErrorHandler { get; }

        public List<UserWrapper> Users { get; set; } = [];

        public List<RoleWrapper> Roles { get; set; } = [];

        public List<PermissionWrapper> Permissions { get; set; } = [];

        /// <summary>
        /// Метод для добавления пользователя, возвращает Id в базе нового пользователя, если добавление прошло херово то вернет -1
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns>возвращает Id в базе нового пользователя, если добавление прошло херово то вернет -1</returns>
        public int AddUser(string name,string password, string? description)
        {
            int result = -1;
            try
            {
                if(Users.FirstOrDefault(x => x.Name == name) != null)
                {
                    return -1;
                }

                using var db = DbContextHelper.GetConstructionContext();
                var user = new User();
                user.Name = name;
                user.Password = password;
                user.Description = description;

                db.Users.Add(user);
                db.SaveChanges();

                ErrorHandler.HandleError($"Пользователь {name} добавлен в базу",Severity.Information);

                var uWrapper = new UserWrapper();
                uWrapper.Id = user.Id;
                uWrapper.Name = name;
                uWrapper.Password = password;
                uWrapper.Description = description ?? "";
                uWrapper.Roles = [];

                Users.Add(uWrapper);

                ErrorHandler.HandleError($"Пользователь {name} добавлен в коллекцию", Severity.Information);

                result = user.Id;
            }
            catch(Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении пользователя {name} -> {e.Message}",Severity.Error);
            }
            return result;
        }

        public bool UpdateUser(UserWrapper wrapper)
        {

            try
            {
                if(Users.FirstOrDefault(x=>x.Id == wrapper.Id) is UserWrapper wr)
                {
                    using var db = DbContextHelper.GetConstructionContext();

                    wr.Name = wrapper.Name;
                    wr.Password = wrapper.Password;
                    wr.Description = wrapper.Description;
                    wr.Roles = [];
                    foreach(var x in wrapper.Roles)
                    {
                        wr.Roles.Add(x);
                    }

                    var user = db.Users.FirstOrDefault(x => x.Id == wrapper.Id);
                    if(user != null)
                    {
                        user.Name = wrapper.Name;
                        user.Password = wrapper.Password;
                        user.Description = wrapper.Description;
                        db.SaveChanges();

                        ErrorHandler.HandleError($"Пользователь {wrapper.Name} обновлен в базе", Severity.Information);

                        var roles = db.RolesOfUsers.Where(x=>x.UserId == wrapper.Id).ToList();
                        foreach(var x in roles)
                        {
                            db.RolesOfUsers.Remove(x);
                        }
                        db.SaveChanges();

                        foreach(var x in wrapper.Roles)
                        {
                            var roleOfUser = new RolesOfUser();
                            roleOfUser.UserId = wrapper.Id;
                            roleOfUser.RoleId = x;
                            db.RolesOfUsers.Add(roleOfUser);
                        }
                        db.SaveChanges();

                        ErrorHandler.HandleError($"Роли пользователя {wrapper.Name} обновлены в базе", Severity.Information);

                        return true;
                    }
                   
                }
                
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при обновлении пользователя {wrapper.Name} -> {e.Message}",Severity.Error);
                
            }
            return false;
        }

        public bool RemoveUser(int userId, string username)
        {
            try
            {
                using var db = DbContextHelper.GetConstructionContext();
                var user = db.Users.FirstOrDefault(x=>x.Id == userId);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();

                    var uWrapper = Users.FirstOrDefault(x => x.Id == userId);

                    if(uWrapper != null)
                    {
                        Users.Remove(uWrapper);
                        ErrorHandler.HandleError($"Пользователь {username} удален",Severity.Information);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при удалении пользователя {username} -> {e.Message}",Severity.Error);
            }
            return false;
        }


        /// <summary>
        /// Метод для добавления м, возвращает Id в базе новой роли, если добавление прошло херово то вернет -1
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns>возвращает Id в базе новой роли, если добавление прошло херово то вернет -1</returns>
        public int AddRole(string name, string description)
        {
            int result = -1;
            try
            {
                var db = DbContextHelper.GetConstructionContext();
                var role = new Role();
                role.Name = name;
                role.Description = description;

                db.Add(role);
                db.SaveChanges();

                var rWrapper = new RoleWrapper();
                rWrapper.Name = name;
                rWrapper.Description = description;
                rWrapper.Permissions = [];
                rWrapper.Id = role.Id;

                Roles.Add(rWrapper);

                ErrorHandler.HandleError($"Добавлена новая роль: {role}",Severity.Information);

                result = role.Id;
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при добавлении роли {name} -> {e.Message}", Severity.Error);
            }
            return result;
        }

        public bool UpdateRole(RoleWrapper wrapper)
        {

            try
            {

            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при обновлении роли {wrapper.Name} -> {e.Message}", Severity.Error);
            }
            return false;
        }
    }
}
