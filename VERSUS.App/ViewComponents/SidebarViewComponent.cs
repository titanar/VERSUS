using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Mvc;

using VERSUS.App.Models;
using VERSUS.Kentico.Types;

namespace VERSUS.App.ViewComponents
{
    public class SidebarViewComponent : SharedViewComponent
    {
        public SidebarViewComponent(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewModel = await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => s.Sidebar.Cast<SidebarItem>())
                .Select(sidebarItems => sidebarItems
                                            .Select(i => new SidebarItemViewModel(i)));

            return View(viewModel);
        }
    }
}