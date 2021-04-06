using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using System.Threading;

namespace Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByUserId
{
    public class GetOrdersByUserIdCommandHandler : AbstractRequestHandler<GetOrdersByUserIdCommand>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrdersByUserIdCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        internal override HandleResponse HandleIt(GetOrdersByUserIdCommand request, CancellationToken cancellationToken)
        {
            var result = _orderRepository.GetOrdersByUserId(request.UserId).Result;

            return new HandleResponse()
            {
                Content = result
            };
        }
    }
}
