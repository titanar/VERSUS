using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using VERSUS.App.Models;
using VERSUS.Kentico.Types;

using IHtmlContent = Microsoft.AspNetCore.Html.IHtmlContent;

namespace VERSUS.App.ViewComponents
{
    public class SidebarComponent : SharedViewComponent
    {
        public SidebarComponent(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        public async Task<IHtmlContent> InvokeAsync()
        {
            var viewModel = await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => s.Sidebar.Cast<SidebarItem>())
                .Select(sidebarItems => sidebarItems
                                            .Select(i => new SidebarItemViewModel(i)));

            return RenderReactComponent(new { model = viewModel }, containerId: "sidebar");
        }
    }
}