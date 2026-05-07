
namespace MinifierTestCore
{


    internal static partial class TestAzureKeyVault
    {

        private static Azure.Security.KeyVault.Secrets.SecretClient 
            GetSecretClient()
        {
            // Your Key Vault URL
            string kvUri = "https://<your-vault-name>.vault.azure.net";
            kvUri = AzureSecrets.KeyVaultUri;

            // InvalidOperationException:
            // "The challenge resource 'vault.azure.net'
            // does not match the requested domain.
            // Set DisableChallengeResourceVerification to true
            // in your client options to disable.
            // See https://aka.ms/azsdk/blog/vault-uri for more information.

            // As a security feature, the SDK blocks requests
            // to unknown domains to prevent your authentication tokens
            // from being accidentally sent to a malicious server.
            // Since you are using an emulator,
            // you need to explicitly tell the SDK to stop this check.
            Azure.Security.KeyVault.Secrets.SecretClientOptions options = 
                new Azure.Security.KeyVault.Secrets.SecretClientOptions()
            {
                DisableChallengeResourceVerification = true
            };

            // Use DefaultAzureCredential for easy authentication
            Azure.Security.KeyVault.Secrets.SecretClient client =
                new Azure.Security.KeyVault.Secrets.SecretClient(
                    new System.Uri(kvUri),
                    // new Azure.Identity.DefaultAzureCredential()
                    new EmulatorCredential(AzureSecrets.EmulatorToken),
                    options
            );

            return client;
        } // End Task GetClient 


        private static Azure.Security.KeyVault.Keys.KeyClient 
            GetKeyClient()
        {
            string kvUri = AzureSecrets.KeyVaultUri;

            // Just like secrets, you must disable challenge verification for the emulator
            Azure.Security.KeyVault.Keys.KeyClientOptions options = 
                new Azure.Security.KeyVault.Keys.KeyClientOptions()
            {
                DisableChallengeResourceVerification = true
            };

            return new Azure.Security.KeyVault.Keys.KeyClient(
                new System.Uri(kvUri),
                new EmulatorCredential(AzureSecrets.EmulatorToken),
                options
            );
        } // End Function GetKeyClient 


        private static Azure.Security.KeyVault.Keys.Cryptography.CryptographyClient 
            GetCryptoClient(System.Uri keyId)
        {

            // 2. Initialize CryptographyClient to use the key
            // We use the ID of the key we just created
            Azure.Security.KeyVault.Keys.Cryptography.CryptographyClient cryptoClient =
                new Azure.Security.KeyVault.Keys.Cryptography.CryptographyClient(
                keyId,
                new EmulatorCredential(AzureSecrets.EmulatorToken),
                new Azure.Security.KeyVault.Keys.Cryptography.CryptographyClientOptions()
                {
                    DisableChallengeResourceVerification = true
                }
            );

            return cryptoClient;
        } // End Function GetCryptoClient 

        public static Azure.Security.KeyVault.Certificates.CertificateClient
            GetCertificateClient()
        {
            // 1. Setup the client (with your emulator options)
            Azure.Security.KeyVault.Certificates.CertificateClientOptions options =
                new Azure.Security.KeyVault.Certificates.CertificateClientOptions()
                {
                    DisableChallengeResourceVerification = true
                };

            Azure.Security.KeyVault.Certificates.CertificateClient client =
                new Azure.Security.KeyVault.Certificates.CertificateClient(
                new System.Uri(AzureSecrets.KeyVaultUri),
                new EmulatorCredential(AzureSecrets.EmulatorToken),
                options
            );

            return client;
        } // End Function GetCertificateClient 


    } // End Class TestAzureKeyVault 


} // End Namespace 
