using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orders.Application.MediatR.Base;
using Orders.Domain.DataAccess.Repositories;
using Orders.Domain.Entities;
using Orders.Domain.Messaging.Email;
using Orders.Domain.Messaging.SNS;
using Orders.Infraestructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orders.Application.Application.MediatR.Commands.Orders.Paypal
{
    public class PaypalCommandHandler : AbstractRequestHandler<PaypalCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly ISnsClient _snsClient;
        private readonly ILogger _logger;

        public PaypalCommandHandler(IConfiguration configuration,
            IOrderRepository orderRepository,
            ISnsClient snsClient,
            ILogger logger)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
            _snsClient = snsClient;
            _logger = logger;
        }

        internal override HandleResponse HandleIt(PaypalCommand request, CancellationToken cancellationToken)
        {
            _logger.Info(request.EventType);

            var eventType = request.EventType.Trim().ToUpperInvariant();
            var externalId = GetExternalId(eventType, request);
            var newExternalId = string.Empty;
            var amount = (decimal)0;

            if (eventType.Equals("PAYMENT.SALE.REFUNDED"))
            {
                amount = Convert.ToDecimal(request.Resource.amount.total) - Convert.ToDecimal(request.Resource.total_refunded_amount.Value);
            }
            else if (eventType.Equals("PAYMENTS.PAYMENT.CREATED"))
            {
                var order = _orderRepository.GetOrdersByFilters(new Dictionary<string, string>() { { "externalId", externalId } }).Result;

                _logger.Info($"{externalId} of {order.Count()}");

                newExternalId = request.Resource.transactions.FirstOrDefault().related_resources.FirstOrDefault().sale.id;
                if (order == null || !order.Any())
                {
                    _logger.Info($"Sending refund email to {newExternalId}");

                    SendRefundMail(newExternalId, request.Resource.summary, request.Resource.payer.payer_info).GetAwaiter().GetResult();

                    return new HandleResponse();
                }
                else if (order.Any() && !string.IsNullOrEmpty(order.FirstOrDefault().PaymentStatus))
                {
                    return new HandleResponse();
                }
            }
            else if (eventType.Equals("PAYMENT.SALE.COMPLETED"))
            {
                var order = _orderRepository.GetOrdersByFilters(new Dictionary<string, string>() { { "externalId", externalId } }).Result;
                if (order.Any() && string.IsNullOrEmpty(order.FirstOrDefault().PaymentStatus))
                {
                    newExternalId = externalId;
                    externalId = request.Resource.parent_payment;
                }
            }

            _orderRepository.UpdateOrder(new Order()
            {
                ExternalId = externalId,
                PaymentStatus = eventType,
                Amount = amount
            }, 
            newExternalId) ;
            

            if (eventType.Equals("CUSTOMER.DISPUTE.CREATED"))
            {
                SendDisputeMail(externalId,
                    request.Resource.summary,
                    request.Resource.reason,
                    request.Resource.dispute_amount.Value)
                    .GetAwaiter()
                    .GetResult();
            }
                
            return new HandleResponse();
        }

        private string GetExternalId(string eventType, PaypalCommand request)
        {
            return eventType switch
            {
                "PAYMENTS.PAYMENT.CREATED" => request.Resource.id,
                "PAYMENT.SALE.COMPLETED" => request.Resource.id,
                "RISK.DISPUTE.CREATED" => request.Resource.disputed_transactions.FirstOrDefault().seller_transaction_id,
                "CUSTOMER.DISPUTE.CREATED" => request.Resource.disputed_transactions.FirstOrDefault().seller_transaction_id,
                "PAYMENT.SALE.REFUNDED" => request.Resource.sale_id,
                "CUSTOMER.DISPUTE.RESOLVED" => request.Resource.disputed_transactions.FirstOrDefault().seller_transaction_id,
                _ => "",
            };
        }

        private async Task SendRefundMail(string externalId, string summary, PayerInfo payer)
        {
            var template = RefundTemplate.GetRefundBody(externalId, summary, payer);

            var message = new Message()
            {
                Body = template,
                To = "support@player2.store",
                Subject = $"[URGENT] An order from PayPal is not registered on PLAYER2: {externalId}!",
                From = "orders@player2.store"
            };

            await _snsClient.Send(_configuration["EMAIL_TOPIC"],
                JsonConvert.SerializeObject(new
                {
                    Message = message
                }))
                .ConfigureAwait(false);
        }

        private async Task SendDisputeMail(string externalId, string summary, string reason, string amount)
        {
            var template = DisputeTemplate.GetDisputeBody(externalId, summary, reason, amount);

            var message = new Message()
            {
                Body = template,
                To = "support@player2.store",
                Subject = $"[URGENT] Dispute has been started for external id {externalId}!"
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
