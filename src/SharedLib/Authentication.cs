using Google.Apis.Auth.OAuth2;
using Google.Apis.Compute.v1;
using Google.Apis.CloudIAP.v1;
using Google.Apis.Util.Store;

namespace SharedLib
{
    public class Authentication : IAuthentication
    {
        public Authentication()
        {

        }

        public async Task<GoogleCredential> CredentialsFromApplication()
        {
            GoogleCredential googleCredential = await GoogleCredential.GetApplicationDefaultAsync().ConfigureAwait(false);
            string[] scopes = new string[] { ComputeService.Scope.Compute, CloudIAPService.Scope.CloudPlatform };

            if (googleCredential.IsCreateScopedRequired)
            {
                googleCredential = googleCredential.CreateScoped(scopes);
            }
            return googleCredential;
        }

        public async Task Oauth2Authentication()
        {
            UserCredential credential;
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets { ClientId = "575023659107-jd6ceer3ht8a06oneknjc16qpe895v69.apps.googleusercontent.com", ClientSecret = "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" },
                new[] { CloudIAPService.Scope.CloudPlatform, ComputeService.Scope.Compute },
                "user",
                CancellationToken.None,
                new FileDataStore("OauthToken"));
        }
    }

    public interface IAuthentication
    {
        public Task<GoogleCredential> CredentialsFromApplication();
        public Task Oauth2Authentication();
    }
}
