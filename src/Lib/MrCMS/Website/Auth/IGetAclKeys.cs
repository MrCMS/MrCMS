using System;
using System.Collections.Generic;

namespace MrCMS.Website.Auth
{
    public interface IGetACLKeys
    {
        IReadOnlyList<string> GetKeys(Type type, string operation);
    }
}