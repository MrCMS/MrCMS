using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Services
{
    public interface ITokenProvider<T>
    {
        IDictionary<string, Func<T, Task<string>>> Tokens { get; }
    }
    public interface ITokenProvider
    {
        IDictionary<string, Func<Task<string>>> Tokens { get; }
    }
}