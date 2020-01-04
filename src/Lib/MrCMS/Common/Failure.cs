using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Common
{
    public class Failure : IResult
    {
        public Failure(params string[] messages)
        {
            Messages = ResultMessagesSanitizer.Sanitize(messages);
        }

        public Failure(IEnumerable<string> messages) : this(messages?.ToArray())
        {
        }

        public Failure(Exception exception)
        {
            var messages = new List<string>();

            var ex = exception;
            do
            {
                messages.Add(ex.Message);
                messages.Add(ex.StackTrace);
                ex = ex.InnerException;
            } while (ex != null);

            Messages = messages;
        }

        public bool Success => false;
        public ICollection<string> Messages { get; }
    }
    public class Failure<T> : Failure, IResult<T>
    {
        public Failure(params string[] messages) : base(messages)
        {
        }

        public Failure(IEnumerable<string> messages) : base(messages)
        {
        }

        public Failure(Exception exception) : base(exception)
        {
        }

        public Failure(T model, params string[] messages) : base(messages)
        {
            Model = model;
        }

        public Failure(T model, IEnumerable<string> messages) : this(model, messages?.ToArray())
        {
        }

        public T Model { get; }
    }
}