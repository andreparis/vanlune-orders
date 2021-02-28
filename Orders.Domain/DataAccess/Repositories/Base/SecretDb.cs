using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Domain.DataAccess.Repositories.Base
{
    public class SecretDb
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string DbInstanceIdentifier { get; set; }
    }
}
