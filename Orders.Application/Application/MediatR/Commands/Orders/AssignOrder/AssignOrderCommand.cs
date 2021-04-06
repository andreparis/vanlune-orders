using MediatR;
using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Application.Application.MediatR.Commands.Orders.AssignOrder
{
    public class AssignOrderCommand : IRequest<Response>
    {
        public int UserId { get; set; }
        public int[] OrdersId { get; set; } 
    }
}
