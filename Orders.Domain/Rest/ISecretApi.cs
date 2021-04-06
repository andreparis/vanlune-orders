using System.Threading.Tasks;

namespace Orders.Domain.Rest
{
    public interface ISecretApi
    {
        Task<string> GetSecretAsync(string secret);
    }
}
