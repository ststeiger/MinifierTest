
namespace MinifierTestCore;


internal class TestAwsSecrets 
{

    internal static Amazon.SecretsManager.AmazonSecretsManagerClient CreateClient()
    {
        // 1. Re-use or re-initialize the client pointed to MiniStack
        Amazon.SecretsManager.AmazonSecretsManagerConfig config = new Amazon.SecretsManager.AmazonSecretsManagerConfig() {
            ServiceURL = "http://localhost:4566",
            AuthenticationRegion = "us-east-1"
        };
        
        // Access key and Secret key can be any string (MiniStack doesn't validate them)
        Amazon.SecretsManager.AmazonSecretsManagerClient client = 
            new Amazon.SecretsManager.AmazonSecretsManagerClient("test", "test", config);
        return client;
    }

    internal static async System.Threading.Tasks.Task GetSecret()
    {
        Amazon.SecretsManager.AmazonSecretsManagerClient client = CreateClient();
        
        // 2. Create the request with your secret's name
        Amazon.SecretsManager.Model.GetSecretValueRequest request = 
            new Amazon.SecretsManager.Model.GetSecretValueRequest() {
            SecretId = "MyDbConnection"
        };

        try 
        {
            // 3. Retrieve the secret
            Amazon.SecretsManager.Model.GetSecretValueResponse? response = await client.GetSecretValueAsync(request);
    
            // 4. Access the value
            if (response.SecretString != null) 
            {
                string mySecret = response.SecretString;
                System.Console.WriteLine($"Retrieved Secret: {mySecret}");
            }
        }
        catch (Amazon.SecretsManager.Model.ResourceNotFoundException) 
        {
            System.Console.WriteLine("The secret was not found in MiniStack.");
        }
        catch (System.Exception ex) 
        {
            System.Console.WriteLine($"Error retrieving secret: {ex.Message}");
        }

    }

    // package AWSSDK.SecretsManager 
    internal static async System.Threading.Tasks.Task CreateSecret()
    {
        Amazon.SecretsManager.AmazonSecretsManagerClient client = CreateClient();
        
        Amazon.SecretsManager.Model.CreateSecretRequest request = 
            new Amazon.SecretsManager.Model.CreateSecretRequest() 
        {
            Name = "MyDbConnection",
            SecretString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"
        };
        
        // This secret will now be saved to the volume defined in step 1
        await client.CreateSecretAsync(request);
    }
}
