using KenticoCloud.Delivery;
using KenticoCloud.Delivery.Rx;

using Microsoft.AspNetCore.Mvc;

namespace VERSUS.App.ViewComponents
{
    public abstract class SharedViewComponent : ViewComponent
    {
        private readonly IDeliveryClient _deliveryClient;

        protected DeliveryObservableProxy DeliveryObservable => new DeliveryObservableProxy(_deliveryClient);

        public SharedViewComponent(IDeliveryClient deliveryClient)
        {
            _deliveryClient = deliveryClient;
        }
    }
}