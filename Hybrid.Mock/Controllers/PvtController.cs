using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hybrid.Mock.Models;
using System.Diagnostics.CodeAnalysis;

namespace Hybrid.Mock.Controllers
{
    [ExcludeFromCodeCoverage]
    public class PvtController : Controller
    {
        private readonly IAmazonCognitoIdentityProvider _amazonCognitoIdentityProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PvtController> _logger;

        public PvtController(IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider, IConfiguration configuration, ILogger<PvtController> logger)
        {
            _amazonCognitoIdentityProvider = amazonCognitoIdentityProvider;
            _configuration = configuration;
            _logger = logger;
        }

        [Route("api/signin")]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> SignIn([FromBody] User user)
        {
            var adminInitiateAuthRequest = new AdminInitiateAuthRequest()
            {
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH, //test
                UserPoolId = _configuration["AWS:Cognito:PoolId"],
                ClientId = _configuration["AWS:Cognito:ClientIdWithoutSecret"]
            };
            adminInitiateAuthRequest.AuthParameters.Add("USERNAME", user.UserId);
            adminInitiateAuthRequest.AuthParameters.Add("PASSWORD", user.Password);
            try
            {
                _logger.LogInformation("Initiating Auth with Cognito");
                var result = await _amazonCognitoIdentityProvider.AdminInitiateAuthAsync(adminInitiateAuthRequest);
                _logger.LogInformation("Cognito Auth responded with response code {httpStatusCode}", result.HttpStatusCode.ToString());
                return Ok(result?.AuthenticationResult?.AccessToken);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    _logger.LogError(e, "Task error calling RapidApiTracerApi Gateway: {message}", e.Message);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unknown error occurred in {0}", nameof(SignIn));
            }

            return StatusCode(500, "");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("api/pvt")]
        public async Task<IActionResult> Pvt()
        {
            _logger.LogInformation("Logged in with user {0} to perform pvt", ControllerContext.HttpContext.User.Identity.Name);
            return Ok(true);
        }
    }
}