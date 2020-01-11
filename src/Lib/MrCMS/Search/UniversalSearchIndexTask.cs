using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Services;
using MrCMS.Tasks;
using Newtonsoft.Json;

namespace MrCMS.Search
{
    public class UniversalSearchIndexTask : AdHocTask, IUniversalSearchIndexTask
    {
        private readonly ISearchConverter _searchConverter;
        private readonly IEventContext _eventContext;
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public UniversalSearchIndexTask(IUniversalSearchIndexManager universalSearchIndexManager,
            ISearchConverter searchConverter, IEventContext eventContext)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
            _searchConverter = searchConverter;
            _eventContext = eventContext;
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

        protected override async Task OnExecute(CancellationToken token)
        {
            var datas = new List<UniversalSearchIndexData> { UniversalSearchIndexData };
            await UniversalSearchActionExecutor.PerformActions(_universalSearchIndexManager, _searchConverter, datas, _eventContext);
        }
    }
}