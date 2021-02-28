using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AutoFixture;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Orders;
using Orders.Application;
using Orders.Domain.Entities;
using Orders.Domain.Entities.DTO;
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
        public void CreateOrder()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var product = _fixture
                .Build<Order>()
                .Create();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n   \"orders\":[\r\n      {\r\n         \"user\":{\r\n     \"phone\":\"123456789\",\r\n            \"name\":\"Pedro\",\r\n            \"email\":\"pedro.rrmaia@live.com\",\r\n            \"Characters\":[\r\n               {\r\n                  \"name\":\"Vancartier\",\r\n                  \"game\":\"WoW\",\r\n                  \"server\":\"Nemesis\"\r\n               }\r\n            ]\r\n         },\r\n         \"status\":1,\r\n         \"Product\":{\r\n            \"variants\":[\r\n               {\r\n                  \"name\":\"Nemesis\",\r\n                  \"factor\":1.1,\r\n                  \"id\":4\r\n               },\r\n               {\r\n                  \"name\":\"Azarlon\",\r\n                  \"factor\":1.2,\r\n                  \"id\":5\r\n               },\r\n               {\r\n                  \"name\":\"Hyjal\",\r\n                  \"factor\":1.05,\r\n                  \"id\":6\r\n               }\r\n            ],\r\n            \"tags\":[\r\n               \"new\",\r\n               \"topSeller\"\r\n            ],\r\n            \"title\":\"Gold 1M\",\r\n            \"description\":\"Buy 1M WoW's Gold!!\",\r\n            \"category\":{\r\n               \"id\":1\r\n            },\r\n            \"sale\":false,\r\n            \"price\":599.99,\r\n            \"quantity\":1,\r\n            \"discount\":0,\r\n            \"Image\":{\r\n               \"src\":\"https:\\/\\/vanlune-site-images.s3.amazonaws.com\\/products\\/1.jpg\"\r\n            },\r\n            \"id\":55\r\n         }\r\n      }\r\n   ]\r\n}")
                .Create();

            _function.CreateOrders(apiContext, lambdaContext.Object);

        }

        //[Fact]
        //public void GetAllPorductsByCategory()
        //{
        //    var lambdaContext = new Mock<ILambdaContext>(); ;
        //    var apiContext = _fixture
        //        .Build<APIGatewayProxyRequest>()
        //        .With(x=>x.QueryStringParameters, new Dictionary<string, string>() { {"category","gold"} })
        //        .Create();

        //    var result = _function.GetAllProductsByCategory(apiContext, lambdaContext.Object);

        //}
    }
}