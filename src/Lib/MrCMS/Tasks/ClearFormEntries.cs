using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Testing.Values;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Tasks
{
    public class ClearFormEntries : SchedulableTask
    {
        public override int Priority => 0;
        private readonly ISession _session;

        public ClearFormEntries(ISession session)
        {
            _session = session;
        }
        

        protected override void OnExecute()
        {
            var formsToCheck = _session.Query<Form>().Where(x => x.DeleteEntriesAfter != null).ToList();

            var toDelete = new List<FormPosting>();

            foreach (var form in formsToCheck)
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-form.DeleteEntriesAfter.GetValueOrDefault());
                toDelete.AddRange(_session.Query<FormPosting>()
                    .Where(x => x.Form.Id == form.Id && x.CreatedOn < cutoffDate));
            }

            if (toDelete.Any())
            {
                _session.Transact(session => toDelete.ForEach(session.Delete));
            }
        }
    }
}