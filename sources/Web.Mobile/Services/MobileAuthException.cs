namespace Web.Mobile.Services;

public sealed class MobileAuthException : Exception
{
    public MobileAuthException(string messageKey)
        : base(messageKey)
    {
    }

    public string MessageKey => Message;
}
