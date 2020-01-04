using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Common
{
    public class Successful : IResult
    {
        public Successful(params string[] messages)
        {
            Messages = ResultMessagesSanitizer.Sanitize(messages);
        }
        public Successful(IEnumerable<string> messages) : this(messages?.ToArray())
        {
        }
        public bool Success => true;
        public ICollection<string> Messages { get; }
    }

    public class Successful<T> : IResult<T>
    {
        public T Model { get; }

        public Successful(T model, params string[] messages)
        {
            Model = model;
            Messages = ResultMessagesSanitizer.Sanitize(messages);
        }
        public Successful(T model, IEnumerable<string> messages) : this(model, messages?.ToArray())
        {
        }

        public bool Success => true;
        public ICollection<string> Messages { get; }
    }
}