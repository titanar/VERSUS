using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;

using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VERSUS.App.Models;
using VERSUS.Infrastructure.Extensions;
using VERSUS.Infrastructure.Services;
using VERSUS.Kentico.Types;

namespace VERSUS.App.Controllers
{
    public class SiteController : SharedController
    {
        public SiteController(IDeliveryClient deliveryClient) : base(deliveryClient)
        {
        }

        [Route("/")]
        public async Task<IActionResult> Index([FromServices] IReviewService reviewService)
        {
            var reviews = await reviewService.GetReviews().ToListAsync();

            ViewBag.Reviews = reviews;

            return await DeliveryObservable
                .GetItemObservable<Site>("site")
                .Select(s => new SiteViewModel(s))
                .ToActionResult(View);
        }

        [Route("/Error/{errorCode?}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(HttpStatusCode errorCode = HttpStatusCode.InternalServerError)
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorCode = errorCode
            });
        }
    }
}