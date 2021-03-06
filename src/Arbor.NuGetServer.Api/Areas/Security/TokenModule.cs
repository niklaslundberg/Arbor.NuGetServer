using System;
using Alphaleonis.Win32.Filesystem;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Autofac;

namespace Arbor.NuGetServer.Api.Areas.Security
{
    public class TokenModule : Module
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public TokenModule(IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TokenGenerator>().AsSelf().SingleInstance();

            string expiration = _keyValueConfiguration[TokenConfiguration.AudienceKey].WithDefault(TokenConfiguration.DefaultExpirationMinutes);
            string audience = _keyValueConfiguration[TokenConfiguration.AudienceKey].WithDefault(TokenConfiguration.DefaultAudience);
            string issuer = _keyValueConfiguration[TokenConfiguration.IssuerKey].WithDefault(TokenConfiguration.DefaultIssuer);
            string keyFile = _keyValueConfiguration[TokenConfiguration.SecurityKeyFileKey];

            if (string.IsNullOrWhiteSpace(keyFile))
            {
                keyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "key.rsa");

                if (!File.Exists(keyFile))
                {
                    RsaKey key = RsaKeyHelper.CreateKey();
                    RsaKeyHelper.WriteKey(key, keyFile);
                }
            }
            else if (!File.Exists(keyFile))
            {
                throw new InvalidOperationException($"The key file {keyFile} does not exist");
            }

            RsaKey rsaKey = RsaKeyHelper.ReadKey(keyFile);


            if (!int.TryParse(expiration, out int expirationMinutes) || expirationMinutes <= 0)
            {
                expirationMinutes = 5;
            }

            var tokenConfiguration = new TokenConfiguration(TimeSpan.FromMinutes(expirationMinutes),
                new TokenAudience(audience),
                new TokenIssuer(issuer),
                rsaKey);

            builder.RegisterInstance(tokenConfiguration);
            builder.RegisterType<TokenValidator>().AsSelf().SingleInstance();
        }
    }
}
