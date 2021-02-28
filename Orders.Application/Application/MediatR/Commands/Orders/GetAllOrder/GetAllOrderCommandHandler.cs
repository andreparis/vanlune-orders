using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Orders.Application.Application.MediatR.Commands.Orders.GetOrder
{
    public class GetAllOrderCommandHandler : AbstractRequestHandler<GetAllOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        public GetAllOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        internal override HandleResponse HandleIt(GetAllOrderCommand request, CancellationToken cancellationToken)
        {
            var result = _orderRepository.GetAllOrders().Result;

            return new HandleResponse()
            {
                Content = result
            };
        }
    }
}
