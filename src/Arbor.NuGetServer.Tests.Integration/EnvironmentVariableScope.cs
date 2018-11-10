using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arbor.NuGetServer.Tests.Integration
{
    public sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly Dictionary<string, string> _oldVariables;
        private readonly Dictionary<string, string> _variables;

        private EnvironmentVariableScope()
        {
        }

        private EnvironmentVariableScope(Dictionary<string, string> variables, Dictionary<string, string> oldVariables)
        {
            _variables = variables;
            _oldVariables = oldVariables;
        }

        public static EnvironmentVariableScope Create(Dictionary<string, string> variables)
        {
            var oldVariables = new Dictionary<string, string>();

            IDictionary environmentVariables = Environment.GetEnvironmentVariables();

            string[] existingKeys = environmentVariables.Keys.OfType<string>().ToArray();

            foreach (KeyValuePair<string, string> keyValuePair in variables)
            {
                if (existingKeys.Contains(keyValuePair.Key, StringComparer.OrdinalIgnoreCase))
                {
                    string oldValue = Environment.GetEnvironmentVariable(keyValuePair.Key);
                    if (oldValue != null)
                    {
                        oldVariables.Add(keyValuePair.Key, oldValue);
                    }
                }
                else
                {
                    oldVariables.Add(keyValuePair.Key, null);
                }
            }

            foreach (KeyValuePair<string, string> keyValuePair in variables)
            {
                Environment.SetEnvironmentVariable(keyValuePair.Key, keyValuePair.Value);
            }

            return new EnvironmentVariableScope(variables, oldVariables);
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, string> keyValuePair in _oldVariables)
            {
                Environment.SetEnvironmentVariable(keyValuePair.Key, keyValuePair.Value);
            }
        }
    }
}
