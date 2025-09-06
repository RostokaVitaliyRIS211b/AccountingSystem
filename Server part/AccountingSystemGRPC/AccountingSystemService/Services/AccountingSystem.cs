using AccountingSystemService;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;

namespace AccountingSystemService.Services
{
    [Authorize]
    public class AccountingService : AccountingSystem.AccountingSystemBase
    {
        public AccountingService() 
        { 

        }
    }
}
