using System;
using NHibernate;

namespace MrCMS.DbConfiguration
{
    public class NotDeletedFilterDisabler : IDisposable
    {
        private readonly ISession _session;
        private readonly bool _filterEnabled;

        public NotDeletedFilterDisabler(ISession session)
        {
            _session = session;

            _filterEnabled = _session.GetEnabledFilter("NotDeletedFilter") != null;
            if (_filterEnabled)
            {
                _session.DisableFilter("NotDeletedFilter");
            }
        }

        public void Dispose()
        {
            if (_filterEnabled)
            {
                _session.EnableFilter("NotDeletedFilter");
            }
        }
    }
}