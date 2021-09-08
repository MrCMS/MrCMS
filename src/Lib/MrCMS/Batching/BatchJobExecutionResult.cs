namespace MrCMS.Batching
{
    public class BatchJobExecutionResult
    {
        private BatchJobExecutionResult()
        {

        }
        public bool Successful { get; private set; }
        public string Message { get; private set; }

        public static BatchJobExecutionResult Success(string message = null)
        {
            return Result(true, message);
        }
        public static BatchJobExecutionResult Failure(string message = null)
        {
            return Result(false, message);
        }

        private static BatchJobExecutionResult Result(bool success, string message)
        {
            return new BatchJobExecutionResult
            {
                Successful = success,
                Message = message
            };
        }
    }
}