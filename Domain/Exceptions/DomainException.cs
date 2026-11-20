namespace Domain.Exceptions;

/// <summary>Exception métier de base.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
