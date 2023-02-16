using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Amazon.Lambda.Core;
using Hyperion;
using Microsoft.AspNetCore.Authentication;

namespace Hybrid.Mock.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private const string LambdaContext = "LambdaContext";
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext httpContext, ILogger<LoggingMiddleware> logger)
        {
            var lambdaContext = httpContext.Items.ContainsKey(LambdaContext)
                ? httpContext.Items[LambdaContext] as ILambdaContext
                : null;

            var awsRequestId = lambdaContext == null ? "lambdacontextisempty" : lambdaContext.AwsRequestId;

            string CorrelationId = Guid.NewGuid().ToString();
            HyperionExtensions.AddSentryTag("correlationId", CorrelationId);

            using (logger.BeginScope("AwsRequestId: {AwsRequestId}, CorrelationId: {correlationId} ", awsRequestId, CorrelationId))
            {
                try
                {
                    //await LogJwtTokenInfo(httpContext, logger);
                    await _next(httpContext);
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        logger.LogError(e, "Global task error: {0}", e.Message);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unhandled exception occured in the application");
                }
                finally
                {
                    await HyperionExtensions.FlushSentryLogAsync();
                }
            }
        }

        public async Task LogJwtTokenInfo(HttpContext httpContext, ILogger logger)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            if (jwtHandler.CanReadToken(httpContext.Request.Headers["Authorization"].ToString()))
            {
                var jwtToken = jwtHandler.ReadJwtToken(httpContext.Request.Headers["Authorization"].ToString());
                logger.LogInformation("JWT Token: {JwtToken}", JsonSerializer.Serialize(jwtToken));
                
                var securityToken = jwtHandler.ReadToken(httpContext.Request.Headers["Authorization"].ToString());
                logger.LogInformation("Security Token: {SecurityToken}", JsonSerializer.Serialize(securityToken));
            }

            string idToken = await httpContext.GetTokenAsync("id_token");
            string accessToken = await httpContext.GetTokenAsync("access_token");

            logger.LogInformation("Authorization: {Authorization}", httpContext.Request.Headers["Authorization"]);
            logger.LogInformation("id_token: {idToken}", idToken);
            logger.LogInformation("access_token: {accessToken}", accessToken);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
