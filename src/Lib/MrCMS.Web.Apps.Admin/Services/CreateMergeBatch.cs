using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Services.WebsiteManagement;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class CreateMergeBatch : ICreateMergeBatch
    {
        private readonly ICreateBatch _createBatch;
        private readonly IControlBatchRun _controlBatchRun;

        public CreateMergeBatch(ICreateBatch createBatch, IControlBatchRun controlBatchRun)
        {
            _createBatch = createBatch;
            _controlBatchRun = controlBatchRun;
        }

        public async Task<bool> CreateBatch(MergeWebpageConfirmationModel model)
        {
            var list = new List<BatchJob>();

            list.AddRange(model.ChangedPages.FindAll(x => x.ParentId == model.Webpage.Id).Select(pageModel => new MoveWebpageBatchJob
            {
                WebpageId = pageModel.Id,
                NewParentId = model.MergedInto?.Id
            }));

            list.AddRange(model.ChangedPages.FindAll(x => x.OldUrl != x.NewUrl).Select(pageModel =>
                new UpdateUrlBatchJob
                {
                    WebpageId = pageModel.Id,
                    NewUrl = pageModel.NewUrl
                }));

            list.Add(new CompleteMergeBatchJob
            {
                WebpageId = model.Webpage.Id,
                MergedIntoId = model.MergedInto.Id
            });

            var result = await _createBatch.Create(list);

            return await _controlBatchRun.Start(result.InitialBatchRun);
        }
    }
}