using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Mvc;

using VERSUS.App.Models;
using VERSUS.Kentico.Extensions;
using VERSUS.Kentico.Types;

namespace VERSUS.App.ViewComponents
{
    public class AnnouncementViewComponent : SharedViewComponent
    {
        public AnnouncementViewComponent(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(AnnouncementLocationEnum announcementLocation)
        {
            var viewModel = await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => s.Announcements.Cast<Announcement>())
                .Select(announcements => announcements
                                            .Where(a => a.System.Codename != CookieConsentViewComponent.COOKIE_CONSENT_ANNOUNCEMENT_CODENAME
                                                        && a.Location.FirstOrDefault().ToEnum<AnnouncementLocationEnum>() == announcementLocation)
                                            .Select(a => new AnnouncementViewModel(a)));

            return View(viewModel);
        }
    }
}