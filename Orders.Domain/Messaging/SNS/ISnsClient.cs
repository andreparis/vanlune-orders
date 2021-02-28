using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.Messaging.SNS
{
    public interface ISnsClient
    {
        Task Send(string topicArn, string message);
    }
}
