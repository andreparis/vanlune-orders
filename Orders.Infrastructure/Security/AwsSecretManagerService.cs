using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Orders.Infraestructure.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Text;

namespace Orders.Infraestructure.Security
{
    public class AwsSecretManagerService : IAwsSecretManagerService
    {
        private readonly AmazonSecretsManagerClient _secretsManagerClient;
        private readonly ILogger _logger;
        private static Dictionary<string, string> _dictionary = new Dictionary<string, string>();

        public AwsSecretManagerService(IConfiguration configuration,
            ILogger logger)
        {
            _logger = logger;

            if (Debugger.IsAttached)
            {
                _secretsManagerClient = new AmazonSecretsManagerClient(new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]), RegionEndpoint.USEast1);
            }
            else
            {
                _secretsManagerClient = new AmazonSecretsManagerClient(RegionEndpoint.USEast1);
            }   
        }

        public string GetSecret(string secretName)
        {

            _logger.Info($"Getting secret of {secretName}");

            if (_dictionary.ContainsKey(secretName)) return _dictionary[secretName];

            string secret = "";

            MemoryStream memoryStream = new MemoryStream();
            GetSecretValueRequest request = new GetSecretValueRequest();
            request.SecretId = secretName;
            request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

            GetSecretValueResponse response = null;

            try
            {
                response = _secretsManagerClient.GetSecretValueAsync(request).Result;
            }
            catch (DecryptionFailureException e)
            {
                // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InternalServiceErrorException e)
            {
                // An error occurred on the server side.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (InvalidParameterException e)
            {
                // You provided an invalid value for a parameter.
                // Deal with the exception here, and/or rethrow at your discretion
                throw;
            }
            catch (InvalidRequestException e)
            {
                // You provided a parameter value that is not valid for the current state of the resource.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (ResourceNotFoundException e)
            {
                // We can't find the resource that you asked for.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }
            catch (System.AggregateException ae)
            {
                // More than one of the above exceptions were triggered.
                // Deal with the exception here, and/or rethrow at your discretion.
                throw;
            }

            // Decrypts secret using the associated KMS CMK.
            // Depending on whether the secret is a string or binary, one of these fields will be populated.
            if (response.SecretString != null)
            {
                secret = response.SecretString;

                _dictionary.Add(secretName, secret);
            }
            else
            {
                memoryStream = response.SecretBinary;
                StreamReader reader = new StreamReader(memoryStream);
                string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            _logger.Info($"Secret is {secret}");

            return secret;
        }
    }

    public class SecretManagerModel
    {        
        public string Username { get; set; }
        public string Password { get; set; }
        public string DbInstanceIdentifier { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Dbname { get; set; }
    }
}
