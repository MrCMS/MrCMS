using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Tasks
{
    public class ClearFormEntries : SchedulableTask
    {
        private readonly IRepository<Form> _formRepository;
        private readonly IRepository<FormPosting> _formPostingRepository;
        public override int Priority => 0;

        public ClearFormEntries(IRepository<Form> formRepository, IRepository<FormPosting> formPostingRepository)
        {
            _formRepository = formRepository;
            _formPostingRepository = formPostingRepository;
        }


        protected override async Task OnExecute(CancellationToken token)
        {
            var formsToCheck = await _formRepository.Query().Where(x => x.DeleteEntriesAfter != null).ToListAsync(token);

            var toDelete = new List<FormPosting>();

            foreach (var form in formsToCheck)
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-form.DeleteEntriesAfter.GetValueOrDefault());
                toDelete.AddRange(_formPostingRepository.Query()
                    .Where(x => x.Form.Id == form.Id && x.CreatedOn < cutoffDate));
            }

            if (toDelete.Any())
            {
                await _formPostingRepository.DeleteRange(toDelete, token);
            }
        }
    }
}