namespace MinifierTestCore;
// Create a custom credential that always returns your emulator's token
public class EmulatorCredential
    : Azure.Core.TokenCredential
{
    private readonly string m_token;

    public EmulatorCredential(string token)
    {
        this.m_token = token;
    } // End Constructor 

    public override System.Threading.Tasks.ValueTask<Azure.Core.AccessToken> 
        GetTokenAsync(
        Azure.Core.TokenRequestContext requestContext
        , System.Threading.CancellationToken cancellationToken
    )
    {
        return new System.Threading.Tasks.ValueTask<Azure.Core.AccessToken>(
            new Azure.Core.AccessToken(this.m_token, System.DateTimeOffset.MaxValue)
        );
    } // End Task GetTokenAsync 

    public override Azure.Core.AccessToken 
        GetToken(
        Azure.Core.TokenRequestContext requestContext, 
        System.Threading.CancellationToken cancellationToken
    )
    {
        return new Azure.Core.AccessToken(this.m_token, System.DateTimeOffset.MaxValue);
    } // End Function GetToken 

} // End Class EmulatorCredential 
