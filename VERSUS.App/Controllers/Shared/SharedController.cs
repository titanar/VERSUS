using KenticoCloud.Delivery;
using KenticoCloud.Delivery.Rx;

using Microsoft.AspNetCore.Mvc;

namespace VERSUS.App.Controllers
{
    public class SharedController : Controller
    {
        private readonly IDeliveryClient _deliveryClient;

        protected DeliveryObservableProxy DeliveryObservable => new DeliveryObservableProxy(_deliveryClient);

        public SharedController(IDeliveryClient deliveryClient)
        {
            _deliveryClient = deliveryClient;
        }
    }
}