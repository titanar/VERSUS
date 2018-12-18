using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Mvc;

using VERSUS.App.Models;
using VERSUS.Kentico.Types;

namespace VERSUS.App.ViewComponents
{
    public class Sidebar : SharedViewComponent
    {
        public Sidebar(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewModel = await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => new SidebarViewModel(s));

            return View(viewModel);
        }
    }
}