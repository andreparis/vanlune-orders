using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using Orders.Domain.Entities;
using Orders.Domain.Entities.Client;
using Orders.Domain.Messaging.Email;
using Orders.Domain.Messaging.SNS;
using Orders.Infraestructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orders.Application.Application.MediatR.Commands.Orders.CreateOrders
{
    public class CreateOrdersCommandHandler : AbstractRequestHandler<CreateOrdersCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly ISnsClient _snsClient;
        private readonly ILogger _logger;

        public CreateOrdersCommandHandler(IConfiguration configuration,
            IOrderRepository orderRepository,
            ISnsClient snsClient,
            ILogger logger)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
            _snsClient = snsClient;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(CreateOrdersCommand request, CancellationToken cancellationToken)
        {
            if (request.Orders.Count() <= 0 ||
                request.Orders.Any(a => !a.IsValid()))
            {
                return new HandleResponse()
                {
                    Error = "You are trying to send invalid products"
                };
            }
                       
            var ordersId = _orderRepository.AddAllAsync(request.Orders).GetAwaiter().GetResult();

            if (ordersId == null || 
                !ordersId.Any())
            {
                return new HandleResponse()
                { 
                    Error = "Not possible create orders! Try again later!"
                };
            }

            SendOrderMail(request.Orders.FirstOrDefault().User, request.Orders).GetAwaiter().GetResult();

            return new HandleResponse() 
            {
                Content = ordersId
            };
        }

        private async Task SendOrderMail(User user, IEnumerable<Order> orders)
        {
            var template = OrdersTemplate.GetOrderBody(orders);

            var message = new Message()
            {                
                Body = template,
                To = user.Email,
                Subject = $"{user.Name}, we have received your order",
                From = "orders@player2.store",
                Bcs = new List<string>() { "support@player2.store" }
            };

            await _snsClient.Send(_configuration["EMAIL_TOPIC"], 
                JsonConvert.SerializeObject(new 
                { 
                    Message = message
                }))
                .ConfigureAwait(false);
        }
    }
}
