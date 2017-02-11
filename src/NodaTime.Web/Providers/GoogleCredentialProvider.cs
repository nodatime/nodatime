// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Google.Apis.Auth.OAuth2;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.IO;
using System.Text;
using static Microsoft.Azure.KeyVault.KeyVaultClient;

namespace NodaTime.Web.Providers
{
    public class GoogleCredentialProvider
    {
        public static GoogleCredential FetchCredential(IConfiguration configuration)
        {
            // Use the default credentials if the environment variable is set.
            if (Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") != null)
            {
                return GoogleCredential.GetApplicationDefaultAsync().Result;
            }
            string secretUri = configuration["APPSETTING_SecretUri"];
            string clientId = configuration["APPSETTING_ClientId"];
            string clientSecret = configuration["APPSETTING_ClientSecret"];
            ClientCredential credential = new ClientCredential(clientId, clientSecret);
            var secret = GetKeyVaultSecret(credential, secretUri);
            return GoogleCredential.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(secret)));
        }

        // See https://docs.microsoft.com/en-us/azure/key-vault/key-vault-use-from-web-application
        private static string GetKeyVaultSecret(ClientCredential credential, string secretUri)
        {
            AuthenticationCallback callback = async (string authority, string resource, string scope) =>
                {
                    var authContext = new AuthenticationContext(authority);
                    AuthenticationResult result = await authContext.AcquireTokenAsync(resource, credential);

                    if (result == null)
                    {
                        throw new InvalidOperationException("Failed to obtain the JWT token");
                    }
                    return result.AccessToken;
                };

            var kv = new KeyVaultClient(callback);
            return kv.GetSecretAsync(secretUri).Result.Value;
        }
    }
}
