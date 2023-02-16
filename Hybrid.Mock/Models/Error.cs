namespace Hybrid.Mock.Models
{
    public class Error
    {
        public string Message { get; set; }

        public static Error Create(string message)
        {
            return new Error { Message = message};
        }

        public static Error ErrorHandler(ILogger logger, Exception exception, string message)
        {
            logger.LogError(exception, message);
            return Create(message);
        }
    }
}
