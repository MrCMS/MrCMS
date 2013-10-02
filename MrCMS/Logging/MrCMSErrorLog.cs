using System;
using System.Collections;
using Elmah;
using MrCMS.Entities.Multisite;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Logging
{
    public class MrCMSErrorLog : ErrorLog, IDisposable
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
                _session = MrCMSApplication.Get<ISessionFactory>().OpenFilteredSession();
        }

        public override string Log(Error error)
        {
            var newGuid = Guid.NewGuid();

            if (_session != null)
            {
                var log = new Log
                              {
                                  Error = error,
                                  Guid = newGuid,
                                  Message = error.Message,
                                  Detail = error.Detail,
                                  Site = _session.Get<Site>(CurrentRequestData.CurrentSite.Id)
                              };
                try
                {
                    _session.Transact(session => session.Save(log));
                }
                catch
                {
                    log.Error = null;
                    _session.Transact(session => session.Save(log));
                }
            }

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
                        .Paged(pageIndex + 1, pageSize);
            errorLogEntries.ForEach(entry =>
                                    errorEntryList.Add(new ErrorLogEntry(this, entry.Guid.ToString(), entry.Error)));
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
                throw new ArgumentException(ex.Message, id, ex);
            }

            try
            {
                var logEntry = _session.QueryOver<Log>().Where(entry => entry.Guid == guid).Cacheable().SingleOrDefault();
                return new ErrorLogEntry(this, id, logEntry.Error);
            }
            finally
            {
            }
        }

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_session != null)
                        _session.Dispose();
                }

                // Indicate that the instance has been disposed.
                _session = null;
                _disposed = true;
            }
        }
    }
}