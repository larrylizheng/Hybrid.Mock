using Amazon.CloudWatchLogs;
using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Hyperion;
using Hyperion.Sentry;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Hybrid.Mock.Core.Data;
using Hybrid.Mock.Core.Infrastructure;
using Hybrid.Mock.Core.Interfaces;
using Hybrid.Mock.Core.Models;
using Hybrid.Mock.Core.Services;
using Hybrid.Mock.Core.Utilities;
using Hybrid.Mock.Infrastructure;
using Hybrid.Mock.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace Hybrid.Mock;

[ExcludeFromCodeCoverage]
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // public IServiceProvider ServiceProvider { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);
        services.AddDefaultAWSOptions(Configuration.GetAWSOptions())
                .AddAWSService<IAmazonCognitoIdentityProvider>();

        services.AddInjectionByConvention("Hybrid.Mock", Assembly.Load("Hybrid.Mock"), typeof(Constants).Assembly);

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(o => o.SessionStore = new MemoryCacheTicketStore())
        .AddOpenIdConnect(options =>
        {
            options.ResponseType = "code";
            options.MetadataAddress = "https://cognito-idp.ap-southeast-2.amazonaws.com/ap-southeast-2_Syqjstc2f/.well-known/openid-configuration";
            options.ClientId = "60j21dgkauefs41eudjpfigco7";
            options.ClientSecret = "169rgmhb2ljnl0m11u8mtbfbkntbs8nhl2o91d78ts2015jaaf88";
            options.SaveTokens = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                //ValidateIssuer = Convert.ToBoolean(Configuration["AWS:OpenId:TokenValidationParameters:ValidateIssuer"]),
                ValidateIssuer = true,
                NameClaimType = "name"
            };
            //options.Events = new OpenIdConnectEvents()
            //{
            //    OnRedirectToIdentityProvider = context =>
            //    {
            //        context.ProtocolMessage.SetParameter("pfidpadapterid", Configuration["oidc:PingProtocolMessage"]);
            //        return Task.FromResult(0);
            //    },
            //    // handle the logout redirection
            //    OnRedirectToIdentityProviderForSignOut = context =>
            //    {
            //        // var logoutUri_test = Configuration["AWS:Cognito:SignedOutRedirectUri"];
            //        var logoutUri = GetCognitoSignOutUri(Configuration);
            //        context.Response.Redirect(logoutUri);
            //        context.HandleResponse();

            //        return Task.CompletedTask;
            //    }
            //};
        })

        // protect web api using JWT tokens - required for
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = Configuration["AWS:Cognito:Authority"],
                ValidateLifetime = true,
                LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                // cannot validate audience as cognito does not send aud key in jwt
                ValidateAudience = false,
                //ValidAudience = Configuration["AWS:Cognito:ClientId"],
                NameClaimType = "username",
                RoleClaimType = "cognito:groups",
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    // Get JsonWebKeySet from AWS
                    var json = new WebClient().DownloadString(parameters.ValidIssuer + "/.well-known/jwks.json");
                    return JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                },
            };
        });

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddConfiguration(Configuration);
        });

        services
            .AddMvc(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
            .SetCompatibilityVersion(CompatibilityVersion.Latest);

        services.AddDataProtection()
            .PersistKeysToAWSSystemsManager($"/{Constants.ApplicationName.ToLowerInvariant()}/DataProtection");
        services.AddRazorPages()
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizePage("/Index");
                options.Conventions.AuthorizePage("/TransactionDetails");
                options.Conventions.AuthorizePage("/PaymentAgreementDetails");
                options.Conventions.AuthorizePage("/Logout");
                options.Conventions.AuthorizePage("/Error");
            });

        services.AddHyperionNLog("Hybrid.Mock");

        services.Configure<AppOptions>(Configuration.GetSection("AppOptions"));
        services.Configure<PayOutCloudWatchOptions>(Configuration.GetSection("PayOutCloudWatchOptions"));
        services.Configure<PayToCloudWatchOptions>(Configuration.GetSection("PayToCloudWatchOptions"));

        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<IDynamoDBContext>(sp =>
        {
            var dynamoDb = sp.GetRequiredService<IAmazonDynamoDB>();
            return new DynamoDBContext(dynamoDb, new DynamoDBContextConfig { TableNamePrefix = GetTablePrefix() });
        });
        services.AddSingleton<IAmazonDynamoDB>(sp => new AmazonDynamoDBClient());
        services.AddAWSService<IAmazonS3>();
        services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client());
        services.AddAWSService<IAmazonCloudWatchLogs>();
        services.AddSingleton<IAmazonCloudWatchLogs>(sp => new AmazonCloudWatchLogsClient());

        services.AddTransient<IPaymentAgreementRepository, PaymentAgreementRespository>();
        services.AddTransient<IPaymentAgreementService, PaymentAgreementService>();
    }

    private static string GetCognitoSignOutUri(IConfiguration configuration)
    {
        return configuration.GetValue<string>("AppOptions:SignOutUri");
    }

    private string GetTablePrefix()
    {
        var configValue = Configuration.GetValue<string>("appOptions:upstreamAvenue") ?? string.Empty;
        var upstreamAvenue = configValue.EndsWith('.') ? configValue : $"{configValue}.";
        return ".".Equals(upstreamAvenue) ? string.Empty : upstreamAvenue;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseXRay(Constants.ApplicationName2);
        app.UseLoggingMiddleWare();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapRazorPages().RequireAuthorization();
            endpoints.MapRazorPages();
            endpoints.MapControllers();
        });

        loggerFactory.CreateLogger<Startup>().LogInformation("Configure completed");
    }
}