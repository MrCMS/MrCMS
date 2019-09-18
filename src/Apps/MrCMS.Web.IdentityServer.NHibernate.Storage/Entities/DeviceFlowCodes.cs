using System;
using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    /// <summary>
    /// Entity for device flow codes
    /// </summary>
    /// <remarks>
    /// The Primary Key field is [UserCode], and maps to ID property in EntityBase.
    /// </remarks>
    public class DeviceFlowCodes : SystemEntity
    {
        //private Guid _guid;

        //protected internal DeviceFlowCodes()
        //{
        //    _guid = Guid.NewGuid();
        //}

       // public virtual int Id { get; set; }

        //public virtual string UserCode
        //{
        //    get { return _guid.ToString(); }
        //}

        public virtual string UserCode { get; set; }

        /// <summary>
        /// Gets or sets the device code.
        /// </summary>
        /// <value>
        /// The device code.
        /// </value>
        public virtual string DeviceCode { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        public virtual string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public virtual string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        public virtual DateTime CreationTime { get; set; }


        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>
        /// The expiration date.
        /// </value>
        public virtual DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public virtual string Data { get; set; }
    }
}
