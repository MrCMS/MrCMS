using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Entities.Documents;
using NHibernate;

namespace MrCMS.Tests.Entities
{
    public class DocumentTypeCount
    {
        public string DocumentType { get; set; }
        public int Total { get; set; }
        public int Unpublished { get; set; }
    }
}