using System;
using NHibernate;

namespace MrCMS.DbConfiguration
{
    public class PublishedFilterDisabler : IDisposable
    {
        private readonly ISession _session;
        private readonly bool _filterEnabled;

        public PublishedFilterDisabler(ISession session)
        {
            _session = session;

            _filterEnabled = _session.GetEnabledFilter("PublishedFilter") != null;
            if (_filterEnabled)
            {
                _session.DisableFilter("PublishedFilter");
            }
        }

        public void Dispose()
        {
            if (_filterEnabled)
            {
                _session.EnableFilter("PublishedFilter");
            }
        }
    }
}