using System.Globalization;

namespace CareerHub.Domain.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException() : base("Something went wrong") { }

    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args)) { }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException) { }

    public override string ToString()
    {
        return $"BadRequestException: {Message}\nStackTrace: {StackTrace}";
    }
}