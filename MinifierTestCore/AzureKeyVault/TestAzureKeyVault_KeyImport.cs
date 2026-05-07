
namespace MinifierTestCore
{


    internal static partial class TestAzureKeyVault
    {


        public static async System.Threading.Tasks.Task 
            TestPfxImport()
        {
            string pfxPath = "/path/to/somepxf.pfx";
            byte[] pfxBytes = await System.IO.File.ReadAllBytesAsync(pfxPath);

            // Optional local verification
            // System.Security.Cryptography.X509Certificates.X509Certificate2 x509_bad =
            //   new System.Security.Cryptography.X509Certificates.X509Certificate2(
            //      pfxBytes,
            //      "password",
            //      System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable
            //);

            System.Security.Cryptography.X509Certificates.X509Certificate2 x509 =
                System.Security.Cryptography.X509Certificates.X509CertificateLoader.LoadPkcs12(
                    pfxBytes,
                    "password",
                    System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable
            );

            await System.Console.Out.WriteLineAsync(x509.Subject);
        } // End Task TestPfxImport 


        public static async System.Threading.Tasks.Task
                ImportFromBouncyCastle(
                Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters bcPrivateKey
            )
        {
            Azure.Security.KeyVault.Keys.KeyClient keyClient =
                GetKeyClient();

            // 1. Create the JsonWebKey object manually
            Azure.Security.KeyVault.Keys.JsonWebKey jwk =
                new Azure.Security.KeyVault.Keys.JsonWebKey(
                new Azure.Security.KeyVault.Keys.KeyOperation[]
            {
                Azure.Security.KeyVault.Keys.KeyOperation.Decrypt,
                Azure.Security.KeyVault.Keys.KeyOperation.Sign,
                Azure.Security.KeyVault.Keys.KeyOperation.UnwrapKey
            })
                {
                    KeyType = Azure.Security.KeyVault.Keys.KeyType.Rsa,

                    // Public parameters
                    N = bcPrivateKey.Modulus.ToByteArrayUnsigned(),
                    E = bcPrivateKey.PublicExponent.ToByteArrayUnsigned(),

                    // Private parameters
                    D = bcPrivateKey.Exponent.ToByteArrayUnsigned(),

                    P = bcPrivateKey.P.ToByteArrayUnsigned(),
                    Q = bcPrivateKey.Q.ToByteArrayUnsigned(),
                    DP = bcPrivateKey.DP.ToByteArrayUnsigned(),
                    DQ = bcPrivateKey.DQ.ToByteArrayUnsigned(),
                    QI = bcPrivateKey.QInv.ToByteArrayUnsigned(),
                };

            // 2. Import to Vault
            await keyClient.ImportKeyAsync("ImportedBCKey", jwk);
        } // End Task ImportFromBouncyCastle 


        // using var rsa = System.Security.Cryptography.RSA.Create(4096);
        public static async System.Threading.Tasks.Task
            ImportLocalRsaToVault(
            System.Security.Cryptography.RSA rsaInstance,
            string keyName
        )
        {
            Azure.Security.KeyVault.Keys.KeyClient keyClient =
                GetKeyClient(); // Uses your existing emulator setup

            // 1. Convert the local RSA object to a JsonWebKey (JWK)
            // The second parameter 'true' includes the private key parameters for the import
            Azure.Security.KeyVault.Keys.JsonWebKey jwk =
                new Azure.Security.KeyVault.Keys.JsonWebKey(
                    rsaInstance,
                    includePrivateParameters: true
            );

            try
            {
                // 2. Upload the JWK to the vault
                Azure.Security.KeyVault.Keys.KeyVaultKey importedKey =
                    await keyClient.ImportKeyAsync(keyName, jwk);

                System.Console.WriteLine($"Key '{importedKey.Name}' successfully imported from local RSA object.");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Import failed: {ex.Message}");
            }
        } // End Task ImportLocalRsaToVault 


        public static async System.Threading.Tasks.Task
            ImportCertificateFromPfx(
            string certName,
            string pfxPath,
            string password
        )
        {
            Azure.Security.KeyVault.Certificates.CertificateClient client = GetCertificateClient();

            // 2. Read the PFX file
            byte[] pfxData = System.IO.File.ReadAllBytes(pfxPath);

            // 3. Import the certificate
            // Password is only needed if the PFX is encrypted
            Azure.Security.KeyVault.Certificates.ImportCertificateOptions importOptions =
                new Azure.Security.KeyVault.Certificates
                .ImportCertificateOptions(certName, pfxData)
                {
                    Password = password
                };

            try
            {
                Azure.Security.KeyVault.Certificates.KeyVaultCertificateWithPolicy cert =
                    await client.ImportCertificateAsync(importOptions);
                System.Console.WriteLine($"Certificate '{cert.Name}' imported successfully.");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Import failed: {ex.Message}");
            }
        } // End Task ImportCertificateFromPfx 


    } // End Class TestAzureKeyVault 


} // End Namespace 
