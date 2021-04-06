using Orders.Infrastructure.Security.Auth.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Orders.Infrastructure.Security
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IsAllowedAttribute : ValidationAttribute
    {
        private readonly IAuthRedis _authRedis;

        public IsAllowedAttribute(IAuthRedis authRedis)
        {
            _authRedis = authRedis;
        }

        public override bool IsValid(object value)
        {
            var inputValue = value as string;


            return !string.IsNullOrEmpty(inputValue);
        }
    }
}
