using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Hybrid.Mock.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class Error
    {
        public string Message { get; set; }

        [JsonIgnore]
        public int HttpStatusCode => (int)ErrorStatusCode;

        [JsonIgnore]
        public HttpStatusCode ErrorStatusCode { get; set; }

        public ErrorType ErrorType { get; set; }

        [JsonIgnore]
        public bool ShouldLogAsError { get; set; }

        private static Error Create(string message, HttpStatusCode errorStatusCode, ErrorType errorType, bool shouldLogAsError = true)
        {
            return new Error() { Message = message, ErrorStatusCode = errorStatusCode, ErrorType = errorType, ShouldLogAsError = shouldLogAsError };
        }

        public static Error Create(ILogger logger, string message, HttpStatusCode errorStatusCode, ErrorType errorType, bool shouldLogAsError = true)
        {
            if (shouldLogAsError)
            {
                logger.LogError(message);
            }
            else
            {
                logger.LogInformation(message);
            }
            return new Error() { Message = message, ErrorStatusCode = errorStatusCode, ErrorType = errorType, ShouldLogAsError = shouldLogAsError };
        }

        public static Error Create(ILogger logger,string message, ErrorType errorType, bool shouldLogAsError = true)
        {
            if (shouldLogAsError)
            {
                logger.LogError(message);
            }
            else
            {
                logger.LogInformation(message);
            }
            return new Error() { Message = message, ErrorType = errorType, ShouldLogAsError = shouldLogAsError };
        }

        public static Error Create( string message, ErrorType errorType, bool shouldLogAsError = true)
        {
            return new Error() { Message = message, ErrorType = errorType, ShouldLogAsError = shouldLogAsError };
        }

        public static Error Unknown(ILogger logger)
        {
            var error = new Error
            {
                Message = "Unknown",
                ErrorType = ErrorType.Retry,
                ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
            };
            logger.LogError("Error: {error}", error);
            return error;
        }

        public static Func<Exception, Error> ErrorHandler(ILogger logger, string message,
            HttpStatusCode errorStatusCode, ErrorType errorType, bool shouldLogAsError = true)
        {
            return exception =>
            {
                if (shouldLogAsError)
                {
                    logger.LogError(exception, message);
                }
                else
                {
                    logger.LogInformation(exception, message);
                }
                return Create(message, errorStatusCode, errorType, shouldLogAsError);
            };
        }

        public static Func<Exception, Error> ErrorHandler(ILogger logger, string message,
            ErrorType errorType, bool shouldLogAsError = true, params object[] args)
        {
            return exception =>
            {
                if (shouldLogAsError)
                {
                    logger.LogError(exception, message, args);
                }
                else
                {
                    logger.LogError(exception, message, args);
                }
                return Create(string.Format(message, args), errorType, shouldLogAsError);
            };
        }

    }

    public enum ErrorType { Unknown, Retry, DoNotRetry }
}
