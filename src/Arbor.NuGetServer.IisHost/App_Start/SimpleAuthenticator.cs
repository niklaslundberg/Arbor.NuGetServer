using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Arbor.NuGetServer.IisHost
{
    public class SimpleAuthenticator
    {
        public Task<IEnumerable<Claim>> IsAuthenticated(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return Task.FromResult<IEnumerable<Claim>>(new List<Claim>());
            }

            var storedUsername = ConfigurationManager.AppSettings["nuget:authentication:basicauthentication:username"];
            var storedPassword = ConfigurationManager.AppSettings["nuget:authentication:basicauthentication:password"];

            var correctUsername = username.Equals(storedUsername, StringComparison.InvariantCultureIgnoreCase);
            var correctPassword = password.Equals(storedPassword, StringComparison.InvariantCulture);

            if (!(correctUsername && correctPassword))
            {
                return Task.FromResult<IEnumerable<Claim>>(new List<Claim>());
            }
            
            return Task.FromResult<IEnumerable<Claim>>(new List<Claim>()
                                                       {
                                                           new Claim(ClaimTypes.NameIdentifier, storedUsername)
                                                       });
        }
    }
}