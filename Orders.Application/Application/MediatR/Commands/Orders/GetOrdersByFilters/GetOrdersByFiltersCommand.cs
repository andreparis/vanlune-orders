using MediatR;
using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByFilters
{
    public class GetOrdersByFiltersCommand : IRequest<Response>
    {
        public IDictionary<string, string> Filters { get; set; }
    }
}
