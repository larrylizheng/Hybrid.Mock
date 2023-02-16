using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Hybrid.Mock.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace Hybrid.Mock.Pages
{
    [ExcludeFromCodeCoverage]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly AppOptions _appOptions;

        public LogoutModel(ILogger<LogoutModel> logger, IOptionsMonitor<AppOptions> appOptions)
        {
            _logger = logger;
            _appOptions = appOptions.CurrentValue;
        }

        public async Task OnGet()
        {
            _logger.LogInformation("Logging out of Hybrid.Mock: {Environment}", _appOptions.Environment);
            _logger.LogInformation("Sign Out Uri: {SignOutUri}", _appOptions.SignOutUri);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task OnPost()
        {
            _logger.LogInformation("Logging out of Hybrid.Mock: {Environment}", _appOptions.Environment);
            _logger.LogInformation("Sign Out Uri: {SignOutUri}", _appOptions.SignOutUri);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}