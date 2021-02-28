using Orders.Application.MediatR.Base;
using Orders.Domain.Entities;
using MediatR;
using Newtonsoft.Json;
using System.Threading;

namespace Orders.Application.MediatR.Events
{
    public class IntegrationEventHandler : AbstractRequestHandler<IntegrationEvent>
    {
        private readonly IMediator _mediator;

        public IntegrationEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        internal override HandleResponse HandleIt(IntegrationEvent request, CancellationToken cancellationToken)
        {
            var @event = GetEvent(request.EventName, request.Message.ToString());

            if (@event == null)
                return null;

            var result = _mediator.Send(@event).Result;

            return new HandleResponse() { Content =  result.Content };
        }

        private IRequest<Response> GetEvent(string eventName, string message)
        {

            return default;
        }
    }
}
