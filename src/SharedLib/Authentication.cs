using Google.Apis.Auth.OAuth2;
using Google.Apis.Compute.v1;

namespace SharedLib
{
    public class Authentication : IAuthentication
    {
        public async Task<GoogleCredential> CredentialsFromApplication()
        {
            GoogleCredential googleCredential = await GoogleCredential.GetApplicationDefaultAsync().ConfigureAwait(false);
            string[] scopes = new string[] { ComputeService.Scope.Compute };

            if (googleCredential.IsCreateScopedRequired)
            {
                googleCredential = googleCredential.CreateScoped(scopes);
            }
            return googleCredential;
        }
    }

    public interface IAuthentication
    {
        public Task<GoogleCredential> CredentialsFromApplication();
    }
}
