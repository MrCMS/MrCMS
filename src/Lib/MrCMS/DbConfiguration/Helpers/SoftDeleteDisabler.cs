using System;
using Microsoft.AspNetCore.Http;

//using MrCMS.Website;

namespace MrCMS.DbConfiguration.Helpers
{
    public class SoftDeleteDisabler : IDisposable
    {
        private readonly HttpContext _context;
        private readonly bool _enableOnDispose;

        public SoftDeleteDisabler(HttpContext context)
        {
            _context = context;
            if (!context.IsSoftDeleteDisabled())
            {
                _enableOnDispose = true;
                context.DisableSoftDelete();
            }
        }
        public void Dispose()
        {
            if (_enableOnDispose)
                _context.EnableSoftDelete();
        }
    }
}