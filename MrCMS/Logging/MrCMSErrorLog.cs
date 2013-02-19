using System;
using System.Collections;
using Elmah;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Logging
{
    public class MrCMSErrorLog : ErrorLog
    {
        private ISession _session;

        public override string Name
        {
            get
            {
                return "MrCMS Database Error Log";
            }
        }

        public MrCMSErrorLog(IDictionary config)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
                _session = MrCMSApplication.Get<ISession>();
        }

        public override string Log(Error error)
        {
            var newGuid = Guid.NewGuid();

            if (_session != null)
                _session.Transact(session => session.Save(new Log
                                                              {
                                                                  Error = error,
                                                                  Guid = newGuid,
                                                                  Message = error.Message,
                                                                  Detail = error.Detail,
                                                                  Site = CurrentRequestData.CurrentSite
                                                              }));

            return newGuid.ToString();
        }

        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, null);
            if (pageSize < 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, null);

            var errorLogEntries =
                _session.QueryOver<Log>()
                        .Where(entry => entry.Type == LogEntryType.Error)
                        .OrderBy(entry => entry.CreatedOn).Desc
                        .Paged(pageIndex + 1, pageSize,
                                    entries =>
                                    entries.Select(entry => new ErrorLogEntry(this, entry.Guid.ToString(), entry.Error)));
            errorLogEntries.ForEach(entry => errorEntryList.Add(entry));
            return errorLogEntries.TotalItemCount;
        }

        public override ErrorLogEntry GetError(string id)
        {
            Guid guid;
            try
            {
                guid = new Guid(id);
                id = guid.ToString();
            }
            catch (FormatException ex)
            {
                throw new ArgumentException(ex.Message, id, (Exception)ex);
            }

            try
            {
                var logEntry = _session.QueryOver<Log>().Where(entry => entry.Guid == guid).Cacheable().SingleOrDefault();
                return new ErrorLogEntry((ErrorLog)this, id, logEntry.Error);
            }
            finally
            {
            }
        }
    }
}