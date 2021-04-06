using Orders.Infraestructure.Logging;
using System.Collections.Generic;
using Orders.Domain.Rest;

namespace Orders.Infraestructure.Security
{
    public class AwsSecretManagerService : IAwsSecretManagerService
    {
        private readonly ISecretApi _secretApi;
        private readonly ILogger _logger;
        private static Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public AwsSecretManagerService(ISecretApi secretApi,
            ILogger logger)
        {
            _logger = logger;
            _secretApi = secretApi;
        }

        public string GetSecret(string secret)
        {
            if (!string.IsNullOrEmpty(secret) && _dictionary.ContainsKey(secret))
            {
                _logger.Info("Key already recovered, returning conection string");
                return _dictionary[secret];
            }

            var secretValue = _secretApi.GetSecretAsync(secret).Result;
            if (!string.IsNullOrEmpty(secretValue))
                _dictionary.Add(secret, secretValue);

            return secretValue;
        }
    }
}
