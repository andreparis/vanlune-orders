using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using System.Threading;

namespace Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByEmail
{
    public class GetOrdersByEmailCommandHandler : AbstractRequestHandler<GetOrdersByEmailCommand>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrdersByEmailCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        internal override HandleResponse HandleIt(GetOrdersByEmailCommand request, CancellationToken cancellationToken)
        {
            var result = _orderRepository.GetOrdersByEmail(request.Email).Result;

            return new HandleResponse()
            {
                Content = result
            };
        }
    }
}
