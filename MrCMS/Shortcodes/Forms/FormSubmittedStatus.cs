namespace MrCMS.Shortcodes.Forms
{
    public struct FormSubmittedStatus
    {
        private readonly bool _submitted;
        private readonly string _error;

        public FormSubmittedStatus(bool submitted, string error)
        {
            _submitted = submitted;
            _error = error;
        }

        public bool Submitted
        {
            get { return _submitted; }
        }

        public string Error
        {
            get { return _error; }
        }

        public bool Success
        {
            get { return Submitted && string.IsNullOrWhiteSpace(Error); }
        }
    }
}