using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core.Extensions;

namespace Arbor.NuGetServer.Core
{
    public class SimpleAuthenticator
    {
        private readonly IKeyValueConfiguration _keyValueConfiguration;

        public SimpleAuthenticator(IKeyValueConfiguration keyValueConfiguration)
        {
            _keyValueConfiguration = keyValueConfiguration;
        }

        public Task<IEnumerable<Claim>> IsAuthenticated(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult<IEnumerable<Claim>>(new List<Claim>());
            }

            var usernameKey = "nuget:authentication:basicauthentication:username";
            var passwordKey = "nuget:authentication:basicauthentication:password";

            string storedUsername =
                _keyValueConfiguration[usernameKey].ThrowIfNullOrWhitespace(
                    $"AppSetting key '{usernameKey}' is not set");
            string storedPassword =
                _keyValueConfiguration[passwordKey].ThrowIfNullOrWhitespace(
                    $"AppSetting key '{passwordKey}' is not set");

            bool correctUsername = username.Equals(storedUsername, StringComparison.InvariantCultureIgnoreCase);
            bool correctPassword = password.Equals(storedPassword, StringComparison.InvariantCulture);

            if (!(correctUsername && correctPassword))
            {
                return Task.FromResult<IEnumerable<Claim>>(new List<Claim>());
            }

            return
                Task.FromResult<IEnumerable<Claim>>(
                    new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, storedUsername),
                            new Claim(ClaimTypes.NameIdentifier, storedUsername)
                        });
        }
    }
}
