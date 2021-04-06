using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AutoFixture;
using Moq;
using Newtonsoft.Json;
using Orders.Application;
using Orders.Domain.Entities;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class Tests
    {
        private Fixture _fixture;
        private Function _function;

        public Tests()
        {
            _fixture = new Fixture();
            _function = new Function();
        }

        [Fact]
        public void CreateRegisteredUserOrderTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var product = _fixture
                .Build<Order>()
                .Create();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\"orders\":[{\"user\":{\"id\":5,\"name\":\"Andre Paris\",\"characters\":[],\"isactive\":true,\"country\":\"Brazil\",\"phone\":\"12312312313\",\"email\":\"andreparis.comp@gmail.com\",\"role\":\"Admin\",\"roleid\":2},\"status\":1,\"product\":{\"ImageSrc\":\"https:\\/\\/vanlune-site-images.s3.amazonaws.com\\/products\\/IAT79NPQWC0Y1586201305793.png\",\"variants\":[{\"name\":\"Azralon\",\"factor\":1,\"Server\":null,\"id\":1}],\"images\":[{\"image_id\":0,\"alt\":\"white\",\"src\":\"https:\\/\\/vanlune-site-images.s3.amazonaws.com\\/products\\/IAT79NPQWC0Y1586201305793.png\",\"variant_id\":null,\"id\":0}],\"tags\":[\"new\"],\"title\":\"World Tour\",\"description\":\"8 mythical dungeons\",\"category\":{\"name\":\"Mythic\",\"description\":null,\"game\":null,\"id\":3},\"sale\":false,\"price\":220,\"quantity\":1,\"discount\":0,\"image\":{\"image_id\":0,\"alt\":null,\"src\":\"https:\\/\\/vanlune-site-images.s3.amazonaws.com\\/products\\/IAT79NPQWC0Y1586201305793.png\",\"variant_id\":null,\"id\":0},\"game\":{\"name\":\"World of Warcraft\",\"id\":1},\"customizes\":[],\"id\":5,\"qty\":1,\"total\":\"220.00\"},\"price\":220,\"quantity\":1,\"variant\":{\"name\":\"Azralon\",\"factor\":1,\"Server\":null,\"id\":1},\"customizes\":[],\"amount\":\"220.00\",\"payment\":1}]}")
                .Create();

            _function.CreateOrders(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void UpdateOrderTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var order = new Order() 
            {
                Id = 8,
                Attendent = new Orders.Domain.Entities.Client.User() 
                {
                    Id = 5
                }
            };
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\"orders\":[{\"id\":9,\"status\":2}]}")
                .Create();

            _function.UpdateOrder(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void UpdatePaypalTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n    \"id\": \"WH-9DA75652JA779282U-77N52484933075035\",\r\n    \"event_version\": \"1.0\",\r\n    \"create_time\": \"2021-03-31T23:18:05.086Z\",\r\n    \"resource_type\": \"sale\",\r\n    \"event_type\": \"PAYMENT.SALE.COMPLETED\",\r\n    \"summary\": \"Payment completed for BRL 589.6 BRL\",\r\n    \"resource\": {\r\n        \"amount\": {\r\n            \"total\": \"589.60\",\r\n            \"currency\": \"BRL\",\r\n            \"details\": {\r\n                \"subtotal\": \"589.60\"\r\n            }\r\n        },\r\n        \"payment_mode\": \"INSTANT_TRANSFER\",\r\n        \"create_time\": \"2021-03-31T23:17:29Z\",\r\n        \"transaction_fee\": {\r\n            \"currency\": \"BRL\",\r\n            \"value\": \"20.45\"\r\n        },\r\n        \"parent_payment\": \"PAYID-MBSQF6Q8A397750LM1614539\",\r\n        \"update_time\": \"2021-03-31T23:17:29Z\",\r\n        \"soft_descriptor\": \"JOHNDOESTES\",\r\n        \"protection_eligibility_type\": \"ITEM_NOT_RECEIVED_ELIGIBLE,UNAUTHORIZED_PAYMENT_ELIGIBLE\",\r\n        \"application_context\": {\r\n            \"related_qualifiers\": [\r\n                {\r\n                    \"id\": \"0FF44637DG086722X\",\r\n                    \"type\": \"CART\"\r\n                }\r\n            ]\r\n        },\r\n        \"protection_eligibility\": \"ELIGIBLE\",\r\n        \"links\": [\r\n            {\r\n                \"method\": \"GET\",\r\n                \"rel\": \"self\",\r\n                \"href\": \"https:\\/\\/api.sandbox.paypal.com\\/v1\\/payments\\/sale\\/5NJ59018LC387451E\"\r\n            },\r\n            {\r\n                \"method\": \"POST\",\r\n                \"rel\": \"refund\",\r\n                \"href\": \"https:\\/\\/api.sandbox.paypal.com\\/v1\\/payments\\/sale\\/5NJ59018LC387451E\\/refund\"\r\n            },\r\n            {\r\n                \"method\": \"GET\",\r\n                \"rel\": \"parent_payment\",\r\n                \"href\": \"https:\\/\\/api.sandbox.paypal.com\\/v1\\/payments\\/payment\\/PAYID-MBSQF6Q8A397750LM1614539\"\r\n            }\r\n        ],\r\n        \"id\": \"609473245M0771511\",\r\n        \"state\": \"completed\",\r\n        \"invoice_number\": \"\"\r\n    },\r\n    \"links\": [\r\n        {\r\n            \"href\": \"https:\\/\\/api.sandbox.paypal.com\\/v1\\/notifications\\/webhooks-events\\/WH-9DA75652JA779282U-77N52484933075035\",\r\n            \"rel\": \"self\",\r\n            \"method\": \"GET\"\r\n        },\r\n        {\r\n            \"href\": \"https:\\/\\/api.sandbox.paypal.com\\/v1\\/notifications\\/webhooks-events\\/WH-9DA75652JA779282U-77N52484933075035\\/resend\",\r\n            \"rel\": \"resend\",\r\n            \"method\": \"POST\"\r\n        }\r\n    ]\r\n}")
                .Create();

            _function.Paypal(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void SignOrderTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\"UserId\":5,\"OrdersId\":[9]}")
                .Create();

            _function.AssignOrders(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void GetAllOrders()
        {
            var lambdaContext = new Mock<ILambdaContext>(); ;
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{}")
                .Create();

            var result = _function.GetAllOrders(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void GetOrdersByEmail()
        {
            var lambdaContext = new Mock<ILambdaContext>(); ;
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, JsonConvert.SerializeObject(new { Email = "pedro.rrmaia@live.com" }))
                .Create();

            var result = _function.GetOrdersByEmail(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void GetOrdersByStatus()
        {
            var lambdaContext = new Mock<ILambdaContext>(); ;
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, JsonConvert.SerializeObject(new { status = 1 }))
                .Create();

            var result = _function.GetOrdersByStatus(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void GetOrdersByUserId()
        {
            var lambdaContext = new Mock<ILambdaContext>(); ;
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, JsonConvert.SerializeObject(new { userId = 5 }))
                .Create();

            var result = _function.GetOrdersByUserId(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void GetOrdersByFilters()
        {
            var lambdaContext = new Mock<ILambdaContext>(); ;
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.QueryStringParameters, 
                new Dictionary<string, string>() { { "startDate", "2021-02-08" }, { "endDate", "2021-04-22" } })
                .Create();

            var result = _function.GetOrdersByFilters(apiContext, lambdaContext.Object);
        }
    }
}