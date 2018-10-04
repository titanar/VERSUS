using System;

using KenticoCloud.Delivery;
using KenticoCloud.Delivery.Rx;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace VERSUS.App.Controllers
{
	public class SharedController : Controller
	{
		private readonly IDeliveryClient deliveryClient;
		private readonly IMemoryCache memoryCache;

		public DeliveryObservableProxy DeliveryObservable => new DeliveryObservableProxy(deliveryClient);

		public SharedController(IDeliveryClient deliveryClient,
								ICodeFirstTypeProvider codeFirstTypeProvider,
								IContentLinkUrlResolver contentLinkUrlResolver,
								IMemoryCache memoryCache)
		{
			this.memoryCache = memoryCache;

			// Use the memory cache for Delivery client
			this.deliveryClient = memoryCache.GetOrCreate(CacheKeys.DeliveryClient, entry =>
			{
				IDeliveryClient cachedDeliveryClient;

				deliveryClient.CodeFirstModelProvider.TypeProvider = codeFirstTypeProvider;
				deliveryClient.ContentLinkUrlResolver = contentLinkUrlResolver;
				cachedDeliveryClient = deliveryClient;

				entry.SlidingExpiration = TimeSpan.FromHours(1);

				return cachedDeliveryClient;
			});
		}
	}
}