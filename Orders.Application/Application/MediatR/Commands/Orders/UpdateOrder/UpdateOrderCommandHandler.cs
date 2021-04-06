using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using Orders.Infraestructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Orders.Application.Application.MediatR.Commands.Orders.UpdateOrder
{
    public class UpdateOrderCommandHandler : AbstractRequestHandler<UpdateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;
        public UpdateOrderCommandHandler(IOrderRepository orderRepository,
            ILogger logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.Orders == null || 
                !request.Orders.Any())
            {
                return new HandleResponse()
                {
                    Error = "You must send an order to be updated!"
                };
            }

            _logger.Info($"Updating {request.Orders.Count()} orders");

            foreach (var order in request.Orders)
            {
                _orderRepository.UpdateOrder(order).GetAwaiter().GetResult();
            }

            return new HandleResponse();
        }
    }
}
