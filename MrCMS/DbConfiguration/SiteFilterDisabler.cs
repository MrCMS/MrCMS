using System;
using MrCMS.Entities.Multisite;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.DbConfiguration
{
    public class SiteFilterDisabler : IDisposable
    {
        private readonly ISession _session;
        private readonly bool _filterEnabled;

        public SiteFilterDisabler(ISession session)
        {
            _session = session;

            _filterEnabled = _session.GetEnabledFilter("SiteFilter") != null;
            if (_filterEnabled)
            {
                _session.DisableFilter("SiteFilter");
            }
        }

        public void Dispose()
        {
            if (_filterEnabled)
            {
                _session.EnableFilter("SiteFilter").SetParameter("site", CurrentRequestData.CurrentSite.Id);
            }
        }
    }
    public class TemporarySiteFilter : IDisposable
    {
        private readonly ISession _session;
        private readonly bool _filterEnabled;

        public TemporarySiteFilter(ISession session, Site site)
        {
            _session = session;

            _filterEnabled = _session.GetEnabledFilter("SiteFilter") != null;
            if (_filterEnabled)
            {
                _session.DisableFilter("SiteFilter");
            }
            _session.EnableFilter("SiteFilter").SetParameter("site", site.Id);
        }

        public void Dispose()
        {
            _session.DisableFilter("SiteFilter");
            if (_filterEnabled)
            {
                _session.EnableFilter("SiteFilter").SetParameter("site", CurrentRequestData.CurrentSite.Id);
            }
        }
    }
}