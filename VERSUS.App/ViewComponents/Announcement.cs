using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Mvc;

using VERSUS.App.Models;
using VERSUS.Kentico.Types;

namespace VERSUS.App.ViewComponents
{
    public class Announcement : SharedViewComponent
    {
        public Announcement(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(AnnouncementTypeEnum announcementType)
        {
            var viewModel = await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => new AnnouncementViewModel(s, announcementType));

            return View(viewModel);
        }
    }
}