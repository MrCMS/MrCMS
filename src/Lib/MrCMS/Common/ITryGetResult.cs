using System;
using System.Threading.Tasks;

namespace MrCMS.Common
{
    public interface ITryGetResult
    {
        IResult<T> GetResult<T>(Func<T> getFunc, string successMessage = null);
        IResult GetResult(Action getFunc, string successMessage = null);
        Task<IResult<T>> GetResultAsync<T>(Func<Task<T>> getFunc, string successMessage = null);
        Task<IResult> GetResultAsync(Func<Task> getFunc, string successMessage = null);
    }
}