using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;

namespace ForestChurches.Components.Configuration
{
    public class Configuration
    {
        private readonly IHostEnvironment _environment;
        private readonly ILogger<Configuration> _logger;
        private readonly IConfiguration _configReader;
        private SecretClient _client;

        public Configuration(ILogger<Configuration> logger, IHostEnvironment environment, IConfiguration configReader)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configReader = configReader ?? throw new ArgumentNullException(nameof(configReader));
        }

        public SecretClient Client
        {
            get
            {
                if (_client == null)
                {
                    AuthenticateKeyVault();
                }

                return _client;
            }
        }
        private void AuthenticateKeyVault()
        {
            var keyVaultURL = _configReader.GetValue<string>("KeyVault:KeyVaultUrl");
            var clientId = _configReader.GetValue<string>("KeyVault:ClientID");
            var tenantId = _configReader.GetValue<string>("KeyVault:DirectoryID");
            var certificatePath = _environment.IsDevelopment() ? _configReader.GetValue<string>("KeyVault:CertificatePath_Development") : _configReader.GetValue<string>("KeyVault:CertificatePath_Production");
            var certificatePassword = _configReader.GetValue<string>("KeyVault:CertificatePassword");

            var certificate = new X509Certificate2(certificatePath, certificatePassword);
            var clientCertificateCredential = new ClientCertificateCredential(tenantId, clientId, certificate);
            _client = new SecretClient(new Uri(keyVaultURL), clientCertificateCredential);
        }
    }
}

// https://germistry.com/Post/1/send-template-emails-with-net-core-mailkit