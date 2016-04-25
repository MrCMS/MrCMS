using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;

namespace MrCMS.Services
{
    public class HashAlgorithmProvider : IHashAlgorithmProvider
    {
        private readonly IDictionary<string, IHashAlgorithm> _hashAlgorithms;

        public HashAlgorithmProvider(IEnumerable<IHashAlgorithm> hashAlgorithms)
        {
            // for performance, we'll use a dictionary, but for a bit of leniency, we'll use a case-insensitive checker
            _hashAlgorithms = hashAlgorithms.GroupBy(x => x.Type)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.First(), StringComparer.OrdinalIgnoreCase);
        }

        public IHashAlgorithm GetHashAlgorithm(string type)
        {
            // we want to first see if the one they've asked for is there, then check to see if a default has been configured.
            // if both of these are unavailable, we drop back onto the default SHA512 implementation
            return GetAlgorithm(type)
                   ?? GetAlgorithm(WebConfigurationManager.AppSettings["HashingMethod"])
                   ?? new SHA512HashAlgorithm();
        }

        private IHashAlgorithm GetAlgorithm(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;
            return _hashAlgorithms.ContainsKey(type)
                ? _hashAlgorithms[type]
                : null;
        }
    }
}