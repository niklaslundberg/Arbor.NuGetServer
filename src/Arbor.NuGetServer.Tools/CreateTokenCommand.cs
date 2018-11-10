using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;
using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Core.Extensions;

namespace Arbor.NuGetServer.Tools
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
            string tenantId = Console.ReadLine().ThrowIfNullOrWhitespace();

            Console.WriteLine("Enter path to key");
            string path = Console.ReadLine().ThrowIfNullOrWhitespace();
            RsaKey rsaKey = RsaKeyHelper.ReadKey(path);

            var tokenGenerator = new TokenGenerator(new TokenConfiguration(TimeSpan.FromMinutes(ttl),
                    new TokenAudience(audience),
                    new TokenIssuer(issuer),
                    rsaKey),
                new CustomSystemClock());

            JwtSecurityToken token = tokenGenerator.CreateJwt(new NuGetTenantId(tenantId),
                new List<NuGetClaimType> { NuGetClaimType.CanReadTenantFeed });

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine("Enter path to save token");
            string tokenPath = Console.ReadLine().ThrowIfNullOrWhitespace();

            File.WriteAllText(tokenPath, jwt, Encoding.UTF8);

            return Task.CompletedTask;
        }
    }
}
