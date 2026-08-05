namespace PAS.Domain;

public class DomainException : Exception {
    public string PropertyName { get; }

    public DomainException() {
        PropertyName = string.Empty;
    }

    public DomainException(string message)
        : base(message) {
        PropertyName = string.Empty;
    }

    public DomainException(string message, string propertyName)
        : base(message) {
        PropertyName = propertyName;
    }

    public DomainException(string message, string propertyName, Exception innerException)
        : base(message, innerException) {
        PropertyName = propertyName;
    }
}
