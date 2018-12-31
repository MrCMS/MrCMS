using System;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate.Engine;

namespace MrCMS.DbConfiguration.Types
{
    [Serializable]
    public class DateTimeData : DateTimeDataBase
    {
        protected override TimeZoneInfo GetTimeZone(ISessionImplementor session)
        {
            return session.GetContext().GetSiteSettings()?.TimeZoneInfo ?? TimeZoneInfo.Local;
        }
    }
}