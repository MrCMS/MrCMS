﻿using System;
using System.Collections;
using Elmah;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Logging
{
    public class MrCMSErrorLog : ErrorLog, IDisposable
    {
        private bool _disposed;
        private ISession _session;

        public MrCMSErrorLog(IDictionary config)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
                _session = MrCMSApplication.Get<ISessionFactory>()
                    .OpenFilteredSession(CurrentRequestData.CurrentContext);
        }

        public override string Name
        {
            get { return "MrCMS Database Error Log"; }
        }

        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        public override string Log(Error error)
        {
            if (_session == null)
                return Guid.NewGuid().ToString();
            var log = new Log
            {
                Error = BinaryData.CanSerialize(error) ? error : new Error(),
                Message = error.Message,
                Detail = error.Detail,
                Site = _session.Get<Site>(CurrentRequestData.CurrentSite.Id)
            };
            _session.Transact(session => session.Save(log));
            return log.Guid.ToString();
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
                var logEntry =
                    _session.QueryOver<Log>().Where(entry => entry.Guid == guid).Cacheable().SingleOrDefault();
                return new ErrorLogEntry(this, id, logEntry.Error);
            }
            finally
            {
            }
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