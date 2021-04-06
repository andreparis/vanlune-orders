using Orders.Domain.Messaging.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Infrastructure.Security.Auth.Redis
{
    public interface IAuthRedis : IRedisClientHelper<Credentials>
    {
        Credentials GetUserClaims(string path);
    }
}
