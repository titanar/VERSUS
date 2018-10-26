using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using VERSUS.App.Models;
using VERSUS.Kentico.Models;

namespace VERSUS.App.Controllers
{
	public class SiteController : SharedController
	{
		public SiteController(IDeliveryClient deliveryClient, ICodeFirstTypeProvider codeFirstTypeProvider, IContentLinkUrlResolver contentLinkUrlResolver, IMemoryCache memoryCache) : base(deliveryClient, codeFirstTypeProvider, contentLinkUrlResolver, memoryCache)
		{
		}

		public async Task<IActionResult> Index()
		{
			var siteObservable = await DeliveryObservable
										.GetItemObservable<Site>("site")
										.Select(s => new SiteViewModel(s));

			return View(siteObservable);
		}

        [Route("Site/Error/{errorCode?}")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int? errorCode = null)
		{
			return View(new ErrorViewModel {
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
				ErrorCode = errorCode
			});
		}
	}
}
