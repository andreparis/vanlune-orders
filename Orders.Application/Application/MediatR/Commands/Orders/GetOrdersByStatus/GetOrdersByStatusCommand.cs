using MediatR;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByStatus
{
    public class GetOrdersByStatusCommand : IRequest<Response>
    {
        public Status Status { get; set; }
    }
}
