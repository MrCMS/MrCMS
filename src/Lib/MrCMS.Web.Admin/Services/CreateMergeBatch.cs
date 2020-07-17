using System.Collections.Generic;
using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Services.WebsiteManagement;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
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

        public bool CreateBatch(MergeWebpageConfirmationModel model)
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

            var result = _createBatch.Create(list);

            return _controlBatchRun.Start(result.InitialBatchRun);
        }
    }
}