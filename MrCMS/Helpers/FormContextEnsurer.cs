using System;
using System.Web.Mvc;

namespace MrCMS.Helpers
{
    public class FormContextEnsurer : IDisposable
    {
        private readonly ViewContext _viewContext;
        private readonly bool _useFakeFormContext;

        public FormContextEnsurer(ViewContext viewContext)
        {
            _viewContext = viewContext;
            if (viewContext.FormContext != null) return;
            _useFakeFormContext = true;
            viewContext.FormContext = new FormContext();
        }

        public void Dispose()
        {
            if (_useFakeFormContext)
                _viewContext.FormContext = null;
        }
    }
}