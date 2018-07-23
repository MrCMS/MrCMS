namespace MrCMS.DbConfiguration.Configuration
{
    // TODO: implement lucene updates
    //public class UpdateIndicesListener : IOnAdded<SiteEntity>, IOnUpdated<SiteEntity>, IOnDeleted<SiteEntity>
    //{
    //    public static void QueueTask(Type type, SiteEntity siteEntity, LuceneOperation operation)
    //    {
    //        if (IndexingHelper.AnyIndexes(siteEntity, operation))
    //        {
    //            var info = new QueuedTaskInfo
    //            {
    //                Data = siteEntity.Id.ToString(),
    //                Type = type.MakeGenericType(siteEntity.GetType()),
    //                SiteId = siteEntity.Site.Id
    //            };

    //            if (!CurrentRequestData.OnEndRequest.OfType<AddLuceneTaskInfo>().Any(task => info.Equals(task.Data)))
    //                CurrentRequestData.OnEndRequest.Add(new AddLuceneTaskInfo(info));
    //        }
    //    }

    //    private static bool ShouldBeUpdated(SiteEntity siteEntity)
    //    {
    //        return siteEntity != null && !(siteEntity is IHaveExecutionStatus);
    //    }

    //    public void Execute(OnAddedArgs<SiteEntity> args)
    //    {
    //        var siteEntity = args.Item;
    //        if (siteEntity == null) return;
    //        if (ShouldBeUpdated(siteEntity))
    //            QueueTask(typeof(InsertIndicesTask<>), siteEntity, LuceneOperation.Insert);
    //    }

    //    public void Execute(OnUpdatedArgs<SiteEntity> args)
    //    {
    //        var siteEntity = args.Item;
    //        if (siteEntity == null) return;
    //        if (ShouldBeUpdated(siteEntity))
    //            QueueTask(typeof(UpdateIndicesTask<>), siteEntity, LuceneOperation.Update);
    //    }

    //    public void Execute(OnDeletedArgs<SiteEntity> args)
    //    {
    //        var siteEntity = args.Item;
    //        if (siteEntity == null) return;
    //        if (ShouldBeUpdated(siteEntity))
    //            QueueTask(typeof(DeleteIndicesTask<>), siteEntity, LuceneOperation.Delete);
    //    }
    //}
}