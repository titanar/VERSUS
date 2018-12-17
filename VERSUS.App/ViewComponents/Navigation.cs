using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Mvc;

using VERSUS.App.Models;
using VERSUS.Kentico.Types;

namespace VERSUS.App.ViewComponents
{
    public class Navigation : SharedViewComponent
    {
        public Navigation(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewModel = await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => new NavigationViewModel(s));

            return View(viewModel);
        }
    }
}