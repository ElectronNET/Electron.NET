namespace ElectronNET.AspNet.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using ElectronNET.AspNet.Services;

    /// <summary>
    /// Middleware that validates authentication for all Electron requests.
    /// Checks for authentication cookie or token query parameter on first request.
    /// Sets HttpOnly cookie for subsequent requests.
    /// 
    /// Security Model:
    /// - First request includes token as query parameter (?token=guid)
    /// - Middleware validates token and sets secure HttpOnly cookie
    /// - Subsequent requests use cookie (no token in URL)
    /// - Both HTTP endpoints and SignalR hub protected
    /// </summary>
    public class ElectronAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IElectronAuthenticationService _authService;
        private readonly ILogger<ElectronAuthenticationMiddleware> _logger;
        private const string AuthCookieName = "ElectronAuth";

        public ElectronAuthenticationMiddleware(
            RequestDelegate next, 
            IElectronAuthenticationService authService,
            ILogger<ElectronAuthenticationMiddleware> logger)
        {
            _next = next;
            _authService = authService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            
            // Check if authentication cookie exists
            var authCookie = context.Request.Cookies[AuthCookieName];
            
            if (!string.IsNullOrEmpty(authCookie))
            {
                // Cookie present - validate it
                if (_authService.ValidateToken(authCookie))
                {
                    await _next(context);
                    return;
                }
                else
                {
                    // Invalid cookie - reject
                    _logger.LogWarning("Authentication failed: Invalid cookie for path {Path} from {RemoteIp}", 
                        path, context.Connection.RemoteIpAddress);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: Invalid authentication");
                    return;
                }
            }

            // No cookie - check for token in query string (first-time authentication)
            var token = context.Request.Query["token"].ToString();
            
            if (!string.IsNullOrEmpty(token))
            {
                if (_authService.ValidateToken(token))
                {
                    // Valid token - set cookie for future requests
                    _logger.LogInformation("Authentication successful: Setting cookie for path {Path}", path);
                    
                    context.Response.Cookies.Append(AuthCookieName, token, new CookieOptions
                    {
                        HttpOnly = true,                     // Prevent JavaScript access (XSS protection)
                        SameSite = SameSiteMode.Strict,      // CSRF protection
                        Path = "/",                          // Valid for all routes
                        Secure = false,                      // False because localhost is HTTP
                        IsEssential = true                   // Required for app to function
                    });
                    
                    await _next(context);
                    return;
                }
                else
                {
                    // Invalid token - reject
                    _logger.LogWarning("Authentication failed: Invalid token (prefix: {TokenPrefix}...) for path {Path} from {RemoteIp}", 
                        token.Length > 8 ? token.Substring(0, 8) : token, path, context.Connection.RemoteIpAddress);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: Invalid authentication");
                    return;
                }
            }

            // Neither cookie nor valid token present - reject
            _logger.LogWarning("Authentication failed: No cookie or token provided for path {Path} from {RemoteIp}", 
                path, context.Connection.RemoteIpAddress);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Authentication required");
        }
    }
}
