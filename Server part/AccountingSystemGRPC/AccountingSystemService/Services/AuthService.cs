using AccountingSystemService.Helpers;
using AccountingSystemService.Interfaces;

using AuthentificationService;

using Grpc.Core;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountingSystemService.Services
{
    public class AuthentificatingService : AuthService.AuthServiceBase
    {
        private IErrorHandler ErrorHandler { get; }
        public AuthentificatingService(IErrorHandler errorHandler)
        {
            ErrorHandler = errorHandler;
        }

        public override Task<AuthReply> Authentificate(AuthRequest request, ServerCallContext context)
        {
            var reply = new AuthReply() { Token = "" };
            try
            {
                using var db = DbContextHelper.GetConstructionContext();
                var user = db.Users.FirstOrDefault(x => x.Name == request.Username);
                if (user is not null && request.Password == user.Password)
                {
                    var rolesOfUser = db.RolesOfUsers.Where(x => x.UserId == user.Id).Select(x => x.RoleId).ToList();
                    var roles = db.Roles.Where(x => rolesOfUser.Contains(x.Id)).Select(x => x.Id).ToList();
                    var permissions = db.PermissionsForRoles.Where(x => roles.Contains(x.RoleId)).Select(x=>x.PermId).ToList();
                    var claims = new List<Claim>
                    {
                        new (ClaimTypes.Name,request.Username),
                        new (ClaimTypes.UserData,Guid.NewGuid().ToString())
                    };
                    foreach(var permission in permissions)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, permission.ToString()));
                    }

                    var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                    var tokenKey = Encoding.ASCII.GetBytes(AuthServiceHelper.JwtKey);
                    var securityTokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature),
                    };
                    var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
                    var token = jwtSecurityTokenHandler.WriteToken(securityToken);
                    if(AuthServiceHelper.Tokens.TryAdd(token, DateTime.UtcNow))
                    {
                        reply.Token = token;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError($"Ошибка при авторизации пользователя {request.Username} -> {Environment.NewLine}{e.Message}", Severity.Error);
            }
            return Task.FromResult(reply);
        }
    }
}
