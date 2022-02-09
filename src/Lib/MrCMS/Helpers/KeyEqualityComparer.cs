using System;
using System.Collections.Generic;

namespace MrCMS.Helpers
{
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        protected readonly Func<T, TKey> KeyExtractor;

        public KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            this.KeyExtractor = keyExtractor;
        }

        public virtual bool Equals(T x, T y)
        {
            return this.KeyExtractor(x).Equals(this.KeyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return this.KeyExtractor(obj).GetHashCode();
        }
    }
}