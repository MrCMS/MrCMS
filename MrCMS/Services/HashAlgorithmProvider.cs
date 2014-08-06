using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Services
{
    public class HashAlgorithmProvider : IHashAlgorithmProvider
    {
        private readonly IEnumerable<IHashAlgorithm> _hashAlgorithms;

        public HashAlgorithmProvider(IEnumerable<IHashAlgorithm> hashAlgorithms)
        {
            _hashAlgorithms = hashAlgorithms;
        }

        public IHashAlgorithm GetHashAlgorithm(string type)
        {
            var hashAlgorithm = _hashAlgorithms.FirstOrDefault(algorithm => algorithm.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            return hashAlgorithm ?? new SHA512HashAlgorithm();
        }
    }
}