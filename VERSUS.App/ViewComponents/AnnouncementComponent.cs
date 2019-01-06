using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using VERSUS.App.Models;
using VERSUS.Kentico.Extensions;
using VERSUS.Kentico.Types;

using IHtmlContent = Microsoft.AspNetCore.Html.IHtmlContent;

namespace VERSUS.App.ViewComponents
{
    public class AnnouncementComponent : SharedViewComponent
    {
        public AnnouncementComponent(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        public async Task<IHtmlContent> InvokeAsync(AnnouncementLocationEnum announcementLocation)
        {
            var viewModel = await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => s.Announcements
                                .Where(a => a.System.Codename != CookieConsentComponent.COOKIE_CONSENT_ANNOUNCEMENT_CODENAME &&
                                            a.Location.FirstOrDefault().ToEnum<AnnouncementLocationEnum>() == announcementLocation)
                                .Select(a => new AnnouncementViewModel(a)));

            return RenderReactComponent(new { model = viewModel });
        }
    }
}