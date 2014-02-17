using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Indexing.Management
{
    //public interface IRelatedItemIndexDefinition<in T1, T2> : IIndexDefinition<T2>
    //    where T2 : SystemEntity
    //    where T1 : SystemEntity
    //{
    //    IEnumerable<T2> GetEntitiesToUpdate(T1 obj);
    //}

    //public interface IIndexDefinition
    //{
    //    /// <summary>
    //    /// Takes your entity, and convert a Lucene document for indexing
    //    /// </summary>
    //    /// <param name="entity">MrCMS entity</param>
    //    /// <returns>a Lucene document ready to be indexed</returns>
    //    Document Convert(object entity);

    //    /// <summary>
    //    /// Retrieves the index term, to allow updates and deletes to be performed on the lucene index, 
    //    /// getting the value from the passed entity (normally the id)
    //    /// </summary>
    //    /// <param name="entity"></param>
    //    /// <returns></returns>
    //    Term GetIndex(object entity);

    //    /// <summary>
    //    /// The name that is shown in admin
    //    /// </summary>
    //    string IndexName { get; }

    //    /// <summary>
    //    /// The name of the type
    //    /// </summary>
    //    string SystemName { get; }

    //    /// <summary>
    //    /// The name used to define the index in the directory
    //    /// </summary>
    //    string IndexFolderName { get; }
    //}

    //public interface IIndexDefinition<T> : IIndexDefinition where T : SystemEntity
    //{
    //    /// <summary>
    //    /// Takes your entity, and convert a Lucene document for indexing
    //    /// </summary>
    //    /// <param name="entity">MrCMS entity</param>
    //    /// <returns>a Lucene document ready to be indexed</returns>
    //    Document Convert(T entity);

    //    /// <summary>
    //    /// Retrieves the index term, to allow updates and deletes to be performed on the lucene index, 
    //    /// getting the value from the passed entity (normally the id)
    //    /// </summary>
    //    /// <param name="entity"></param>
    //    /// <returns></returns>
    //    Term GetIndex(T entity);

    //    /// <summary>
    //    /// Takes a Lucene document and converts it back to the indexed MrCMS entity
    //    /// </summary>
    //    /// <param name="session">NHibernate session to facilitate MrCMS entity loading</param>
    //    /// <param name="document">Lucene document</param>
    //    /// <returns>MrCMS entity</returns>
    //    T Convert(Document document);

    //    /// <summary>
    //    /// Takes a list of Lucene documents and converts it back to a list of indexed MrCMS entities
    //    /// </summary>
    //    /// <param name="session">NHibernate session to facilitate MrCMS entity loading</param>
    //    /// <param name="documents">Lucene documents</param>
    //    /// <returns>MrCMS entities</returns>
    //    IEnumerable<T> Convert(IEnumerable<Document> documents);

    //    /// <summary>
    //    /// Declares the location of the folder within the file system where the index is stored
    //    /// </summary>
    //    /// <param name="currentSite">Site of the request to allow folder paths to be different per site</param>
    //    /// <returns>FOlder location</returns>
    //    string GetLocation(Site currentSite);

    //    /// <summary>
    //    /// Defines the analyzer to use when indexing entities
    //    /// </summary>
    //    /// <returns>An Analyzer</returns>
    //    Analyzer GetAnalyser();

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    IEnumerable<FieldDefinition<T>> Definitions { get; }
    //}
}