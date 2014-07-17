using System;
using System.Collections.Generic;

namespace MrCMS.Helpers
{
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        protected readonly Func<T, TKey> KeyExtractor;

        public KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            KeyExtractor = keyExtractor;
        }

        public virtual bool Equals(T x, T y)
        {
            return KeyExtractor(x).Equals(KeyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return KeyExtractor(obj).GetHashCode();
        }
    }
}