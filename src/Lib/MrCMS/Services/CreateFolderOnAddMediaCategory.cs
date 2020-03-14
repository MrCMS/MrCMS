using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Events;

namespace MrCMS.Services
{
    public class CreateFolderOnAddMediaCategory : OnDataAdded<MediaCategory>
    {
        private readonly IRepository<MediaCategory> _repository;
        private readonly IFileService _fileService;

        public CreateFolderOnAddMediaCategory(IRepository<MediaCategory> repository, IFileService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }

        //public void Execute(OnAddedArgs<MediaCategory> args)
        //{
        //    var mediaCategory = args.Item;

        //    _fileService.CreateFolder(mediaCategory);
        //}

        public override async Task Execute(EntityData data)
        {
            var mediaCategory = await _repository.GetData(data.EntityId);

            await _fileService.CreateFolder(mediaCategory);
        }
    }
}