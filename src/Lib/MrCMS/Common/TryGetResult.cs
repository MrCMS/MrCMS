using System;
using System.Threading.Tasks;

namespace MrCMS.Common
{
    public class TryGetResult : ITryGetResult
    {
        public IResult<T> GetResult<T>(Func<T> getFunc, string successMessage = null)
        {
            try
            {
                return new Successful<T>(getFunc(), successMessage);
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }
        public IResult GetResult(Action getFunc, string successMessage = null)
        {
            try
            {
                getFunc();
                return new Successful(successMessage);
            }
            catch (Exception ex)
            {
                return new Failure(ex);
            }
        }
        public async Task<IResult<T>> GetResultAsync<T>(Func<Task<T>> getFunc, string successMessage = null)
        {
            try
            {
                return new Successful<T>(await getFunc(), successMessage);
            }
            catch (Exception ex)
            {
                return new Failure<T>(ex);
            }
        }
        public async Task<IResult> GetResultAsync(Func<Task> getFunc, string successMessage = null)
        {
            try
            {
                await getFunc();
                return new Successful(successMessage);
            }
            catch (Exception ex)
            {
                return new Failure(ex);
            }
        }
    }
}