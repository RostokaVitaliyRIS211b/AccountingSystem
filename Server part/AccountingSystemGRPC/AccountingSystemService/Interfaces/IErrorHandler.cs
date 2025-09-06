namespace AccountingSystemService.Interfaces
{
    public interface IErrorHandler
    {
        public void HandleError(string message, Severity severity);
    }

    public enum Severity
    {
        Information = 0,
        Warning,
        Error,
    }
}
