using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.Security;
using Arbor.NuGetServer.Api.Areas.Time;

namespace Arbor.NuGetServer.Tools.Commands
{
    internal class CreateTokenCommand : AppCommand
    {
        public override Task RunAsync()
        {
            Console.WriteLine($"Enter audience, default {TokenConfiguration.DefaultAudience}");
            string audience = Console.ReadLine().WithDefault(TokenConfiguration.DefaultAudience);

            Console.WriteLine($"Enter issuer, default {TokenConfiguration.DefaultIssuer}");
            string issuer = Console.ReadLine().WithDefault(TokenConfiguration.DefaultIssuer);

            const int defaultTtl = 5;
            Console.WriteLine($"Enter TTL in minutes, default {defaultTtl}");
            string ttlAttemptedValue = Console.ReadLine();

            int ttl = int.TryParse(ttlAttemptedValue, out int ttlValue) && ttlValue > 0 ? ttlValue : defaultTtl;
            Console.WriteLine($"Using TTL {ttl} minutes");

            Console.WriteLine("Enter tenant id");
            string tenantId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(tenantId))
            {
                Console.WriteLine("Invalid tenant id");
                return Task.CompletedTask;
            }

            Console.WriteLine("Enter path to key");
            string path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("No key path entered");
                return Task.CompletedTask;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine($"Key path '{path}' does not exist");
                return Task.CompletedTask;
            }

            RsaKey rsaKey = RsaKeyHelper.ReadKey(path);

            var tokenGenerator = new TokenGenerator(new TokenConfiguration(TimeSpan.FromMinutes(ttl),
                    new TokenAudience(audience),
                    new TokenIssuer(issuer),
                    rsaKey),
                new CustomSystemClock());

            var nuGetTenantId = new NuGetTenantId(tenantId);

            Console.WriteLine("Enter path to save token");
            string tokenPath = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(tokenPath))
            {
                JwtSecurityToken token = tokenGenerator.CreateJwt(nuGetTenantId,
                    new List<NuGetClaimType> { NuGetClaimType.CanReadTenantFeed });

                string jwt = new JwtSecurityTokenHandler().WriteToken(token);

                TrySaveFile(tokenPath, jwt);
            }

            Console.WriteLine("Enter path to save API token");
            string apiTokenPath = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(apiTokenPath))
            {
                JwtSecurityToken apiToken = tokenGenerator.CreateJwt(nuGetTenantId,
                    new List<NuGetClaimType> { NuGetClaimType.CanReadTenantFeed, NuGetClaimType.CanPushPackage });

                string apiJwt = new JwtSecurityTokenHandler().WriteToken(apiToken);

                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(apiJwt));

                TrySaveFile(apiTokenPath,$"Base64{Environment.NewLine}{base64}{Environment.NewLine}{Environment.NewLine}JWT{Environment.NewLine}{apiJwt}");
            }

            return Task.CompletedTask;
        }

        private static void TrySaveFile(string tokenPath, string jwt)
        {
            try
            {
                File.WriteAllText(tokenPath, jwt, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not save token to file path '{tokenPath}': Exception: " + ex.Message);
            }
        }
    }
}
