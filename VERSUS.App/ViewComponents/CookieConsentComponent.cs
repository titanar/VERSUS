using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Http.Features;

using VERSUS.App.Models;
using VERSUS.Kentico.Types;

using IHtmlContent = Microsoft.AspNetCore.Html.IHtmlContent;

namespace VERSUS.App.ViewComponents
{
    public class CookieConsentComponent : SharedViewComponent
    {
        public static string COOKIE_CONSENT_ANNOUNCEMENT_CODENAME = "cookie_consent";

        public CookieConsentComponent(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        public async Task<IHtmlContent> InvokeAsync()
        {
            var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
            var showBanner = !consentFeature?.CanTrack ?? false;
            string cookieString = null;

            if (showBanner)
            {
                cookieString = consentFeature?.CreateConsentCookie();
            }

            var viewModel = await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => s.Announcements
                            .Where(a => a.System.Codename == COOKIE_CONSENT_ANNOUNCEMENT_CODENAME)
                            .Select(a => new CookieConsentViewModel(a, cookieString))
                            .FirstOrDefault());

            return RenderReactComponent(new { model = viewModel });
        }
    }
}