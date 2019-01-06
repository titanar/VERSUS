using System;

using KenticoCloud.Delivery;
using KenticoCloud.Delivery.Rx;

using Microsoft.AspNetCore.Mvc;

using React.AspNet;

using IHtmlContent = Microsoft.AspNetCore.Html.IHtmlContent;

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

        protected IHtmlContent RenderReactComponent<TModel>(
            TModel props,
            string componentName = null,
            string htmlTag = null,
            string containerId = null,
            string containerClass = null,
            bool clientOnly = false,
            Action<Exception, string, string> exceptionHandler = null
        )
        {
            return HtmlHelperExtensions.React(null, componentName ?? $"Versus.{GetType().Name}", props, htmlTag, containerId, clientOnly, false, containerClass, exceptionHandler);
        }
    }
}