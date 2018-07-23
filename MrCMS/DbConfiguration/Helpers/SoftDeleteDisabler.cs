using System;
//using MrCMS.Website;

namespace MrCMS.DbConfiguration.Helpers
{
    // TODO: enable soft delete
    public class SoftDeleteDisabler : IDisposable
    {
        private readonly bool _enableOnDispose;

        public SoftDeleteDisabler()
        {
            //if (!CurrentRequestData.CurrentContext.IsSoftDeleteDisabled())
            //{
            //    _enableOnDispose = true;
            //    CurrentRequestData.CurrentContext.DisableSoftDelete();
            //}
        }
        public void Dispose()
        {
            //if (_enableOnDispose)
            //    CurrentRequestData.CurrentContext.EnableSoftDelete();
        }
    }
}