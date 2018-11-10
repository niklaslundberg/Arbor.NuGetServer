using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using IdentityModel;
using JetBrains.Annotations;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using File = Alphaleonis.Win32.Filesystem.File;

namespace Arbor.NuGetServer.Abstractions
{
    public static class RsaKeyHelper
    {
        public static RsaSecurityKey ConvertToRsaSecurityKey([NotNull] this RsaKey rsaKey)
        {
            if (rsaKey == null)
            {
                throw new ArgumentNullException(nameof(rsaKey));
            }

            return new RsaSecurityKey(rsaKey.Parameters);
        }

        public static RsaKey ReadKey(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"The file {fileName} does not exist");
            }

            string json = File.ReadAllText(fileName, Encoding.UTF8);

            var rsaSecurityKey = JsonConvert.DeserializeObject<RsaKey>(json,
                new JsonSerializerSettings { ContractResolver = new RsaKeyContractResolver() });

            return rsaSecurityKey;
        }

        public static void WriteKey([NotNull] RsaKey rsaKey, string fileName)
        {
            if (rsaKey == null)
            {
                throw new ArgumentNullException(nameof(rsaKey));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));
            }

            string json = ConvertToJson(rsaKey);

            File.WriteAllText(fileName,
                json,
                Encoding.UTF8);
        }

        public static string ConvertToJson([NotNull] RsaKey rsaKey)
        {
            if (rsaKey == null)
            {
                throw new ArgumentNullException(nameof(rsaKey));
            }

            return JsonConvert.SerializeObject(rsaKey,
                new JsonSerializerSettings
                {
                    ContractResolver = new RsaKeyContractResolver(),
                    Formatting = Formatting.Indented
                });
        }

        public static RsaKey CreateKey()
        {
            RsaSecurityKey key = CreateRsaSecurityKey();

            RSAParameters parameters = key.Rsa?.ExportParameters(true) ?? key.Parameters;

            var rsaKey = new RsaKey
            {
                Parameters = parameters,
                KeyId = key.KeyId
            };

            return rsaKey;
        }

        private static RsaSecurityKey CreateRsaSecurityKey()
        {
            RsaSecurityKey key;
            using (RSA rsa = RSA.Create())
            {
                int keyLength = 2048;

                if (rsa is RSACryptoServiceProvider)
                {
                    rsa.Dispose();
                    var cng = new RSACng(keyLength);

                    RSAParameters parameters = cng.ExportParameters(true);
                    key = new RsaSecurityKey(parameters);
                }
                else
                {
                    rsa.KeySize = keyLength;
                    key = new RsaSecurityKey(rsa);
                }
            }

            key.KeyId = CryptoRandom.CreateUniqueId(16);

            return key;
        }
    }
}
