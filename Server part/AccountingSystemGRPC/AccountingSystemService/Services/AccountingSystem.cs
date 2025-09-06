using AccountingSystemService;
using AccountingSystemService.Interfaces;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;

namespace AccountingSystemService.Services
{
    [Authorize]
    public class AccountingService : AccountingSystem.AccountingSystemBase
    {
        private IErrorHandler ErrorHandler { get; set; }
        public AccountingService(IErrorHandler errorHandler) 
        { 
            ErrorHandler = errorHandler;
        }
    }
}
