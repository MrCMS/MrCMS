using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports
{

    public class UserSubscriptionReportsSearchQuery
    {
        private DateTime _startDate;
        private DateTime _endDate;

        public UserSubscriptionReportsSearchQuery()
        {
            _startDate = DateTime.MinValue;
            _endDate = DateTime.MinValue;
            Subscriptions = new D3UserSubscriptionReportsModel();
        }
        [Required(ErrorMessage = "*start date")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public String StartDate
        {
            get
            {
                if (_startDate == DateTime.MinValue)
                {
                    _startDate = new DateTime(DateTime.Now.Year, 1, 1);
                    return String.Format("{0:MM/dd/yyyy}", _startDate);         
                }
                else
                    return String.Format("{0:MM/dd/yyyy}", _startDate); 
            }
            set
            {
                _startDate = Convert.ToDateTime(value);
            }
        }
        [Required(ErrorMessage = "*end date")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public String EndDate
        {
            get
            {
                if (_endDate == DateTime.MinValue)
                {
                    _endDate = DateTime.Now;
                    return String.Format("{0:MM/dd/yyyy}", _endDate);    
                }
                else
                    return String.Format("{0:MM/dd/yyyy}", _endDate);
            }
            set
            {
                _endDate = Convert.ToDateTime(value);
            }
        }

        public D3UserSubscriptionReportsModel Subscriptions { get; set; }
    }
}