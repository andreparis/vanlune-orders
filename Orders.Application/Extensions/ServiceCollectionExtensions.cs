using Orders.Infraestructure.Logging;
using Orders.Infraestructure.Security;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System;
using Orders.Infrastructure.DataAccess.Database.Base;
using Orders.Domain.DataAccess.Repositories;
using Orders.Infrastructure.Messaging.SNS;
using Orders.Domain.Messaging.SNS;
using Orders.Infrastructure.DataAccess.Database;
using StackExchange.Redis.Extensions.Core.Configuration;
using System.Collections.Generic;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Implementations;
using System.Diagnostics;
using Orders.Domain.Rest;
using Orders.Infrastructure.Rest;

namespace Orders.Application.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            IConfigurationRoot configuration = GetConfiguration();
            services.AddSingleton<IConfiguration>(configuration);

#if DEBUG
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
#endif
            services.AddMediatR(AppDomain.CurrentDomain.Load("Orders.Application"));
            services.AddAutoMapper(typeof(Function).Assembly);
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IAwsSecretManagerService, AwsSecretManagerService>();

            services.AddSingleton<IMySqlConnHelper, MySqlConnHelper>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<ISecretApi, SecretApi>();
            services.AddSingleton<ISnsClient, SnsClient>();

            services.AddHttpClient();

            return services;
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile($"appsettings.json")
                            .AddEnvironmentVariables();

            var configuration = builder.Build();
            return configuration;
        }
    }
}
