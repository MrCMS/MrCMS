using System;
using System.Collections.Generic;

namespace MrCMS.Helpers
{
    public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        protected readonly Func<T, TKey> keyExtractor;

        public KeyEqualityComparer(Func<T, TKey> keyExtractor)
        {
            this.keyExtractor = keyExtractor;
        }

        public virtual bool Equals(T x, T y)
        {
            return this.keyExtractor(x).Equals(this.keyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return this.keyExtractor(obj).GetHashCode();
        }
    }
}