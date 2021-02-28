using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Orders.Domain.DataAccess.Repositories;
using Orders.Domain.DataAccess.Repositories.Base;
using Orders.Infraestructure.Security;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Orders.Infrastructure.DataAccess.Database.Base
{
    public class MySqlConnHelper : IMySqlConnHelper
    {
        private readonly string _connectionString;

        public MySqlConnHelper(IConfiguration configuration,
            IAwsSecretManagerService awsSecretManagerService)
        {
            var secret = JsonConvert.DeserializeObject<SecretDb>(awsSecretManagerService.GetSecret(configuration["CONN_STRING"]));
            _connectionString = $@"server={secret.Host};
                                userid={secret.Username};
                                password={secret.Password};
                                userid=usr_vanlune_dev;
                                password=FJL6ftJdha6jmh!;
                                database=Vanlune;
                                Pooling=True;
                                Min Pool Size=0;
                                Max Pool Size=5;
                                Connection Lifetime=60; 
                                default command timeout=300;";
        }

        public DbConnection MySqlConnection()
        {
            return new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
        }
    }
}
