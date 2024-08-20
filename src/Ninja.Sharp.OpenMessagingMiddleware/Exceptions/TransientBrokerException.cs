namespace Ninja.Sharp.OpenMessagingMiddleware.Exceptions
{
    public class TransientBrokerException : Exception
    {
        public TransientBrokerException(string message) : base(message)
        {
        }

        public TransientBrokerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
