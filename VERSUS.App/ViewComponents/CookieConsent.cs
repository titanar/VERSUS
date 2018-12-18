using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using VERSUS.App.Models;

namespace VERSUS.App.ViewComponents
{
    public class CookieConsent : ViewComponent
    {
        public IViewComponentResult Invoke(string header)
        {
            var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
            var showBanner = !consentFeature?.CanTrack ?? false;
            string cookieString = null;

            if (showBanner)
            {
                cookieString = consentFeature?.CreateConsentCookie();
            }

            return View(new CookieConsentViewModel
            {
                Show = showBanner,
                Header = header ?? "We use cookies",
                CookieString = cookieString
            });
        }
    }
}