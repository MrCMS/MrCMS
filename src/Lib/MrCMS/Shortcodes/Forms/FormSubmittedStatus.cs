using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MrCMS.Shortcodes.Forms
{
    public struct FormSubmittedStatus
    {
        private readonly bool _submitted;
        private readonly List<string> _errors;
        private readonly NameValueCollection _data;

        public FormSubmittedStatus(bool submitted, List<string> errors, NameValueCollection data)
        {
            _submitted = submitted;
            _errors = errors ?? new List<string>();
            _data = data ?? new NameValueCollection();
        }

        public bool Submitted
        {
            get { return _submitted; }
        }

        public List<string> Errors
        {
            get { return _errors; }
        }

        public bool Success
        {
            get { return Submitted && !Errors.Any(); }
        }

        public NameValueCollection Data
        {
            get { return _data; }
        }
    }
}