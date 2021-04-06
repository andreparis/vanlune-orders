using System.Threading.Tasks;

namespace Orders.Domain.Rest
{
    public interface ISnsApi
    {
        Task<string> SendEmailAsyn(string topicArn, string message);
    }
}
