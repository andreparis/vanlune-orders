using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Infraestructure.Security
{
    public interface IAwsSecretManagerService
    {
        string GetSecret(string secret);
    }
}
