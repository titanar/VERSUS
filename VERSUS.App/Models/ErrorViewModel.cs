using System.Net;

namespace VERSUS.App.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public HttpStatusCode ErrorCode { get; set; }
    }
}