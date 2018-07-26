using System.Collections.Generic;
using MrCMS.Tasks;
using Newtonsoft.Json;

namespace MrCMS.Search
{
    public class UniversalSearchIndexTask : AdHocTask, IUniversalSearchIndexTask
    {
        private readonly ISearchConverter _searchConverter;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public UniversalSearchIndexTask(IUniversalSearchIndexManager universalSearchIndexManager,
            ISearchConverter searchConverter)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
            _searchConverter = searchConverter;
        }

        public override int Priority
        {
            get { return 100; }
        }

        public UniversalSearchIndexData UniversalSearchIndexData { get; set; }

        public override string GetData()
        {
            return JsonConvert.SerializeObject(UniversalSearchIndexData);
        }

        public override void SetData(string data)
        {
            UniversalSearchIndexData = JsonConvert.DeserializeObject<UniversalSearchIndexData>(data);
        }

        protected override void OnExecute()
        {
            var datas = new List<UniversalSearchIndexData> {UniversalSearchIndexData};
            UniversalSearchActionExecutor.PerformActions(_universalSearchIndexManager, _searchConverter, datas);
        }
    }
}