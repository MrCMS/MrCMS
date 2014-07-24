using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Indexing.Management;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    public class LuceneActionExecutor
    {
        public static void PerformActions(IIndexService indexService, List<LuceneAction> luceneActions)
        {
            foreach (var @group in luceneActions.GroupBy(action => action.Type))
            {
                var managerBase = indexService.GetIndexManagerBase(@group.Key);
                if (!managerBase.IndexExists)
                    managerBase.CreateIndex();

                IGrouping<Type, LuceneAction> thisGroup = @group;
                managerBase.Write(writer =>
                                  {
                                      foreach (
                                          var luceneAction in
                                              thisGroup.Where(action => action.Operation == LuceneOperation.Insert).ToList())
                                          luceneAction.Execute(writer);
                                      foreach (
                                          var luceneAction in
                                              thisGroup.Where(action => action.Operation == LuceneOperation.Update).ToList())
                                          luceneAction.Execute(writer);
                                      foreach (
                                          var luceneAction in
                                              thisGroup.Where(action => action.Operation == LuceneOperation.Delete).ToList())
                                          luceneAction.Execute(writer);

                                      writer.Optimize();
                                  });
            }
        }
    }
}