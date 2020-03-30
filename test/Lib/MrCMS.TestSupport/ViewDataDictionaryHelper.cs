using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MrCMS.TestSupport
{
    public static class ViewDataDictionaryHelper
    {
        public static ViewDataDictionary GetNewDictionary()
        {
            return new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        }
    }
    
}