using System.Collections.Generic;

namespace MrCMS.Common
{
    public interface IResult
    {
        bool Success { get; }
        ICollection<string> Messages { get; }
    }
    public interface IResult<out T> : IResult {
        T Model { get; }
    }
}