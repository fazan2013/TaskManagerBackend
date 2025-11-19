using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TaskManager.Api.Auth
{
    // Register with: AddAuthentication("Basic").AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", null);
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public BasicAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 1. Check Authorization header
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return AuthenticateResult.Fail("Invalid Authorization Scheme");
                }

                if (string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    return AuthenticateResult.Fail("Missing credentials");
                }

                // 2. Decode credentials: "username:password"
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes);

                var separatorIndex = credentials.IndexOf(':');
                if (separatorIndex == -1)
                {
                    return AuthenticateResult.Fail("Invalid Authorization Header format");
                }

                var username = credentials.Substring(0, separatorIndex);
                var password = credentials.Substring(separatorIndex + 1);

                // 3. Validate credentials (here from appsettings.json)
                if (!ValidateCredentials(username, password))
                {
                    return AuthenticateResult.Fail("Invalid username or password");
                }

                // 4. Create claims & principal
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (FormatException)
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred during basic authentication.");
                return AuthenticateResult.Fail("An error occurred while authenticating");
            }
        }

        /// <summary>
        /// Reads valid username/password from configuration and compares.
        /// appsettings.json:
        /// "BasicAuth": {
        ///   "Username": "admin",
        ///   "Password": "password123"
        /// }
        /// </summary>
        private bool ValidateCredentials(string username, string password)
        {
            var validUsername = _configuration["BasicAuth:Username"];
            var validPassword = _configuration["BasicAuth:Password"];

            if (string.IsNullOrWhiteSpace(validUsername) || string.IsNullOrWhiteSpace(validPassword))
            {
                // If not configured, fail authentication.
                Logger.LogWarning("BasicAuth credentials are not configured in appsettings.");
                return false;
            }

            // Simple comparison for assignment purposes
            return string.Equals(username, validUsername, StringComparison.Ordinal) &&
                   string.Equals(password, validPassword, StringComparison.Ordinal);
        }

        // Optional: customize challenge so browser shows login dialog / client knows it's Basic
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "Basic realm=\"TaskManager\"";
            return base.HandleChallengeAsync(properties);
        }
    }
}
