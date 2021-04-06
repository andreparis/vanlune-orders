using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Orders.Application.Extensions;
using MediatR;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;
using System.Net;
using Orders.Application.Application.MediatR.Commands.Orders.CreateOrders;
using Orders.Application.Application.MediatR.Commands.Orders.GetOrder;
using Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByEmail;
using Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByStatus;
using Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByUserId;
using Orders.Domain.Enums;
using Orders.Application.Application.MediatR.Commands.Orders.GetOrdersByFilters;
using Orders.Application.Application.MediatR.Commands.Orders.UpdateOrder;
using Orders.Application.Application.MediatR.Commands.Orders.AssignOrder;
using Orders.Application.Application.MediatR.Commands.Orders.Paypal;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Orders.Application
{
    public class Function
    {
        protected IServiceProvider _serviceProvider = null;
        protected ServiceCollection _serviceCollection = new ServiceCollection();
        protected IMediator _mediator;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            ConfigureServices();
            _mediator = _serviceProvider.GetService<IMediator>();
        }

        #region APIs

        #region Orders

        public APIGatewayProxyResponse CreateOrders(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"CreateOrders started");

            return Request<CreateOrdersCommand>(request.Body);
        }

        public APIGatewayProxyResponse UpdateOrder(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"UpdateOrder started");

            return Request<UpdateOrderCommand>(request.Body);
        }

        public APIGatewayProxyResponse AssignOrders(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"UpdateOrder started");

            return Request<AssignOrderCommand>(request.Body);
        }

        public APIGatewayProxyResponse GetOrdersByFilters(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"GetOrdersByFilters query");

            var command = new GetOrdersByFiltersCommand()
            {
                Filters = request.QueryStringParameters
            };

            return MediatrSend(command);
        }

        public APIGatewayProxyResponse GetAllOrders(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"GetAllOrderCommand started");

            return Request<GetAllOrderCommand>(request.Body);
        }

        public APIGatewayProxyResponse GetOrdersByEmail(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"CreateOrders started");

            var email = request.QueryStringParameters["email"];

            if (string.IsNullOrEmpty(email))
                return Response("You must inform email!");

            var command = new GetOrdersByEmailCommand() { Email = email };

            return MediatrSend(command);
        }

        public APIGatewayProxyResponse GetOrdersByStatus(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"CreateOrders started");

            var status = request.QueryStringParameters["status"];

            if (string.IsNullOrEmpty(status))
                return Response("You must inform email!");

            var command = new GetOrdersByStatusCommand() { Status = (Status)Convert.ToInt32(status) };

            return MediatrSend(command);
        }

        public APIGatewayProxyResponse GetOrdersByUserId(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"CreateOrders started");

            var user = request.QueryStringParameters["user"];

            if (string.IsNullOrEmpty(user))
                return Response("You must inform email!");

            var command = new GetOrdersByUserIdCommand() { UserId = Convert.ToInt32(user) };

            return MediatrSend(command);
        }

        public APIGatewayProxyResponse Paypal(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"PaypalCommand started");

            return Request<PaypalCommand>(request.Body);
        }

        #endregion


        #endregion

        #region Private Methods
        private void SqsResquest<T>(SQSEvent sqsEvent, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"Beginning to process {sqsEvent.Records.Count} records...");

            foreach (var record in sqsEvent.Records)
            {
                var message = JsonConvert.DeserializeObject<T>(record.Body);

                _mediator.Send(message);
            }

            lambdaContext.Logger.LogLine("Processing complete.");

            lambdaContext.Logger.LogLine($"Processed {sqsEvent.Records.Count} records.");
        }
        private APIGatewayProxyResponse Request<T>(string body)
        {
            Console.WriteLine("body is "+ body);

            var request = JsonConvert.DeserializeObject<T>(body);
            return MediatrSend<T>(request);
        }

        private APIGatewayProxyResponse MediatrSend<T>(T request)
        {
            var result = _mediator.Send(request).Result;
            return Response(JsonConvert.SerializeObject(result));
        }

        private APIGatewayProxyResponse Response(string message)
        {
            var header = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" }
            };

            return new APIGatewayProxyResponse
            {
                Headers = header,
                Body = message,
                StatusCode = (int)HttpStatusCode.OK
            };
        }
        
        private void ConfigureServices()
        {
            _serviceCollection.AddDependencies();
            _serviceProvider = _serviceCollection.BuildServiceProvider();

            _mediator = _serviceProvider.GetService<IMediator>();
        }
        #endregion
    }
}
