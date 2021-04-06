using Orders.Infraestructure.Logging;
using Orders.Infraestructure.Messaging.Redis;
using Orders.Infrastructure.Security.Auth.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Orders.Infrastructure.Security.Auth
{
    public class AuthRedis : RedisClientHelper<Credentials>, IAuthRedis
    {
        private const string SECRET = "secret:login";
        private const string PREFIX = "users:player2:login:";

        public AuthRedis(IRedisCacheClient cacheClient,
            ILogger logger) : base(cacheClient, logger) { }

        public Credentials GetUserClaims(string path)
        {
            var key = string.Concat(PREFIX, path);

            return GetFromRedis(key);
        }
    }
}
