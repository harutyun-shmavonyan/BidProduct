using System.Net;
using Newtonsoft.Json;

namespace BidProduct.API.Middlewares
{
    public class ExceptionResult
    {
        [JsonIgnore] public HttpStatusCode StatusCode { get; set; }
        [JsonIgnore] public string ContentType { get; } = Middlewares.ContentType.Json;
        public string? Message { get; set; }
        public string? Reason { get; set; }
        public string? TraceId { get; set; }
    }
}