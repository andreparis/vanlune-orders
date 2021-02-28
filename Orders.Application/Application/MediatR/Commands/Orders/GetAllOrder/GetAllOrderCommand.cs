using MediatR;
using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Application.Application.MediatR.Commands.Orders.GetOrder
{
    public class GetOrderCommand : IRequest<Response>
    {
    }
}
