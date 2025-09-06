using AccountingSystemService.Interfaces;

using System.Diagnostics;

namespace AccountingSystemService.Realization
{
    public class ErrorHandlerA : IErrorHandler
    {
        public void HandleError(string message, Severity severity)
        {
            if(severity == Severity.Error)
            {
                Trace.TraceError(message);
            }
            else if(severity == Severity.Warning)
            {
                Trace.TraceWarning(message);
            }
            else if(severity == Severity.Information)
            {
                Trace.TraceInformation(message);
            }
        }
    }
}
