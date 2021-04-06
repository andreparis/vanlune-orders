using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Orders.Application.Application.MediatR.Commands.Orders.AssignOrder
{
    public class AssignOrderCommandHandler : AbstractRequestHandler<AssignOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        public AssignOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        internal override HandleResponse HandleIt(AssignOrderCommand request, CancellationToken cancellationToken)
        {
            foreach(var id in request.OrdersId)
            {
                _orderRepository.UpdateOrder(new Domain.Entities.Order() 
                {
                    Id = id,
                    Status = Domain.Enums.Status.InService,
                    Attendent = new Domain.Entities.Client.User()
                    {
                        Id = request.UserId
                    }                    
                }).GetAwaiter().GetResult();
            }

            return new HandleResponse();
        }
    }
}
