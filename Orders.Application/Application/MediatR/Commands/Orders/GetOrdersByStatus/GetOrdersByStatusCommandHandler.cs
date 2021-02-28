using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using System.Threading;

namespace Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByStatus
{
    public class GetOrdersByStatusCommandHandler : AbstractRequestHandler<GetOrdersByStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrdersByStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        internal override HandleResponse HandleIt(GetOrdersByStatusCommand request, CancellationToken cancellationToken)
        {
            var result = _orderRepository.GetOrdersByStatus(request.Status).Result;

            return new HandleResponse()
            {
                Content = result
            };
        }
    }
}
