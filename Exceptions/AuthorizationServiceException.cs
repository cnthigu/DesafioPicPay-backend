namespace PicPayClone.Exceptions
{
    public class AuthorizationServiceException : Exception
    {
        public AuthorizationServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}