using System;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Application.Entities
{
    [MrCMSMapClass]
    public class Order : BaseEntity
    {
        public virtual string Email { get; set; }

        public virtual string ShippingFirstName { get; set; }
        public virtual string ShippingLastName { get; set; }
        public virtual string ShippingAddress1 { get; set; }
        public virtual string ShippingAddress2 { get; set; }
        public virtual string ShippingAddress3 { get; set; }
        public virtual string ShippingCity { get; set; }
        public virtual string ShippingCounty { get; set; }
        public virtual string ShippingPostcode { get; set; }
        public virtual string ShippingTelephone { get; set; }
        public virtual string ShippingCountry { get; set; }

        public virtual bool BillingAddressSameAsShippingAddress { get; set; }

        public virtual string BillingFirstName { get; set; }
        public virtual string BillingLastName { get; set; }
        public virtual string BillingAddress1 { get; set; }
        public virtual string BillingAddress2 { get; set; }
        public virtual string BillingAddress3 { get; set; }
        public virtual string BillingCity { get; set; }
        public virtual string BillingCounty { get; set; }
        public virtual string BillingPostcode { get; set; }
        public virtual string BillingTelephone { get; set; }
        public virtual string BillingCountry { get; set; }

        public virtual Guid SessionGuid { get; set; }

        public virtual bool IsComplete { get; set; }

        public virtual Site Site { get; set; }

    }
}