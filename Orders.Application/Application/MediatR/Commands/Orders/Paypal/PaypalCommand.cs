using MediatR;
using Newtonsoft.Json;
using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Application.Application.MediatR.Commands.Orders.Paypal
{
    public class PaypalCommand : IRequest<Response>
    {
        public string Id { get; set; }
        [JsonProperty("event_version")]
        public string EventVersion { get; set; }
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }
        [JsonProperty("resource_type")]
        public string ResourceType { get; set; }
        [JsonProperty("event_type")]
        public string EventType { get; set; }
        public string Summary { get; set; }
        public Resource Resource { get; set; }
        public List<Link> Links { get; set; }
    }
}
