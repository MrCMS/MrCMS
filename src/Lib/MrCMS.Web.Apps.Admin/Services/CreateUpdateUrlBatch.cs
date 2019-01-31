using System.Linq;
using MrCMS.Batching.Services;
using MrCMS.Services.WebsiteManagement;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class CreateUpdateUrlBatch : ICreateUpdateUrlBatch
    {
        private readonly ICreateBatch _createBatch;
        private readonly IControlBatchRun _controlBatchRun;

        public CreateUpdateUrlBatch(ICreateBatch createBatch, IControlBatchRun controlBatchRun)
        {
            _createBatch = createBatch;
            _controlBatchRun = controlBatchRun;
        }

        public bool CreateBatch(MoveWebpageConfirmationModel model)
        {
            var itemsToUpdate = model.ChangedPages.FindAll(x => x.OldUrl != x.NewUrl);

            if (!itemsToUpdate.Any())
            {
                return true;
            }

            var result = _createBatch.Create(itemsToUpdate.Select(x => new UpdateUrlBatchJob
            {
                NewUrl = x.NewUrl,
                WebpageId = x.Id
            }));

            return _controlBatchRun.Start(result.InitialBatchRun);
        }
    }
}