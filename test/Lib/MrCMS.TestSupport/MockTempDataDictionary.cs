using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MrCMS.TestSupport
{
    public class MockTempDataDictionary : Dictionary<string, object>, ITempDataDictionary
    {
        public void Load()
        {
        }

        public void Save()
        {
        }

        public void Keep()
        {
        }

        public void Keep(string key)
        {
        }

        public object Peek(string key)
        {
            return ContainsKey(key) ? this[key] : null;
        }
    }
}