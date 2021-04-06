using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByFilters
{
    class GetOrdersByFiltersCommandHandler : AbstractRequestHandler<GetOrdersByFiltersCommand>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrdersByFiltersCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        internal override HandleResponse HandleIt(GetOrdersByFiltersCommand request, CancellationToken cancellationToken)
        {
            if (request.Filters == null ||
                request.Filters.Keys.Count == 0)
            {
                return new HandleResponse() 
                {
                    Error = "No filters found!"
                };
            }

            var result = _orderRepository.GetOrdersByFilters(request.Filters).Result;

            return new HandleResponse()
            {
                Content = result
            };
        }
    }
}
