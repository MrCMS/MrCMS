using System;
using System.Collections.Concurrent;

namespace MrCMS.Website.Auth
{
    public class AclKeyGenerator : IAclKeyGenerator
    {
        // Use Nested ConcurrentDictionaries for thread safety
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> Cache = new();

        public static string GetKey(string rule, string operation)
        {
            // Get or create the nested dictionary for the given rule
            var ruleDictionary = Cache.GetOrAdd(rule, new ConcurrentDictionary<string, string>());

            // Get or add the concatenated string in the nested dictionary
            return ruleDictionary.GetOrAdd(operation, ValueFactory(rule));
        }

        private static Func<string, string> ValueFactory(string rule)
        {
            return operationKey => $"{rule}.{operationKey}";
        }

        string IAclKeyGenerator.GetKey(string rule, string operation)
        {
            return GetKey(rule, operation);
        }
    }
}