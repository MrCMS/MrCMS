using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching.Services;
using MrCMS.Services.WebsiteManagement;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
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

        public async Task<bool> CreateBatch(MoveWebpageConfirmationModel model)
        {
            var itemsToUpdate = model.ChangedPages.FindAll(x => x.OldUrl != x.NewUrl);

            if (!itemsToUpdate.Any())
            {
                return true;
            }

            var result = await _createBatch.Create(itemsToUpdate.Select(x => new UpdateUrlBatchJob
            {
                NewUrl = x.NewUrl,
                WebpageId = x.Id
            }));

            return await _controlBatchRun.Start(result.InitialBatchRun);
        }
    }
}