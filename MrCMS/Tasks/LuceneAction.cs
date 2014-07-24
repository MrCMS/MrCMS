using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;
using MrCMS.Indexing.Management;

namespace MrCMS.Tasks
{
    public class LuceneAction
    {
        public LuceneOperation Operation { get; set; }

        public IndexDefinition IndexDefinition { get; set; }
        public SystemEntity Entity { get; set; }

        public Type Type
        {
            get { return IndexDefinition.GetType(); }
        }

        public void Execute(IndexWriter writer)
        {
            Document document = IndexDefinition.Convert(Entity);
            Term index = IndexDefinition.GetIndex(Entity);
            if (document == null || index == null)
                return;
            switch (Operation)
            {
                case LuceneOperation.Insert:
                    writer.AddDocument(document);
                    break;
                case LuceneOperation.Update:
                    writer.UpdateDocument(index, document);
                    break;
                case LuceneOperation.Delete:
                    writer.DeleteDocuments(index);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}