using System;

using KenticoCloud.Delivery;
using KenticoCloud.Delivery.Rx;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using VERSUS.Core;

namespace VERSUS.App.Controllers
{
	public class SharedController : Controller
	{
        private readonly IMemoryCache memoryCache;

        private IDeliveryClient DeliveryClient { get; }

        protected DeliveryObservableProxy DeliveryObservable => new DeliveryObservableProxy(DeliveryClient);

		public SharedController(IDeliveryClient deliveryClient, IMemoryCache memoryCache)
		{
			this.memoryCache = memoryCache;

			// Use the memory cache for Delivery client
			DeliveryClient = memoryCache.GetOrCreate(VersusConstants.CACHE_DeliveryClient, entry =>
			{
				entry.SlidingExpiration = TimeSpan.FromHours(1);

				return deliveryClient;
			});
		}
	}
}