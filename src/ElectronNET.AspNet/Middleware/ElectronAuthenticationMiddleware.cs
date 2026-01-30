namespace ElectronNET.AspNet.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using ElectronNET.AspNet.Services;

    /// <summary>
    /// Middleware that validates authentication for all Electron requests.
    /// Checks for authentication cookie or token query parameter on first request.
    /// Sets HttpOnly cookie for subsequent requests.
    /// </summary>
    public class ElectronAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IElectronAuthenticationService _authService;
        private const string AuthCookieName = "ElectronAuth";

        public ElectronAuthenticationMiddleware(RequestDelegate next, IElectronAuthenticationService authService)
        {
            _next = next;
            _authService = authService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for SignalR negotiation (will be handled by cookie)
            if (context.Request.Path.StartsWithSegments("/electron-hub/negotiate"))
            {
                await _next(context);
                return;
            }

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
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: Invalid authentication cookie");
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
            }

            // Neither cookie nor valid token present - reject
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Authentication required");
        }
    }
}
