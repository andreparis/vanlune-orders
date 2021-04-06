using MediatR;
using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Application.Application.MediatR.Commands.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest<Response>
    {
        public IEnumerable<Order> Orders { get;set; }
    }
}
