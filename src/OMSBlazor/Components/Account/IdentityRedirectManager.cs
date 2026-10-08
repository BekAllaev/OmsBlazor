using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace OMSBlazor.Components.Account
{
    internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
    {
        public const string StatusCookieName = "Identity.StatusMessage";

        private static readonly CookieBuilder StatusCookieBuilder = new()
        {
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            IsEssential = true,
            MaxAge = TimeSpan.FromSeconds(5),
        };

        [DoesNotReturn]
        public void RedirectTo(string? uri)
        {
            uri ??= "";

            // Prevent open redirects.
            if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                uri = ToLocalRelativePath(uri);
            }

            // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
            // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
            navigationManager.NavigateTo(uri);
            throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
        }

        [DoesNotReturn]
        public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
        {
            var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
            var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
            RedirectTo(newUri);
        }

        [DoesNotReturn]
        public void RedirectToWithStatus(string uri, string message, HttpContext context)
        {
            context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
            RedirectTo(uri);
        }

        // ToBaseRelativePath throws when the URI doesn't start with BaseUri. Behind a proxy (Azure App Service)
        // the server may see the request as http while the browser sends https URLs, so the same-site
        // returnUrl would crash the login with 500. Compare by host only and drop anything off-site.
        private string ToLocalRelativePath(string uri)
        {
            var baseUri = new Uri(navigationManager.BaseUri);

            if (!Uri.TryCreate(uri, UriKind.Absolute, out var absoluteUri)
                || !string.Equals(absoluteUri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase))
            {
                return "";
            }

            var localUri = new UriBuilder(absoluteUri) { Scheme = baseUri.Scheme, Port = baseUri.Port }.Uri.AbsoluteUri;

            return localUri.StartsWith(baseUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase)
                ? localUri[baseUri.AbsoluteUri.Length..]
                : "";
        }

        private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

        [DoesNotReturn]
        public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
            => RedirectToWithStatus(CurrentPath, message, context);
    }
}
