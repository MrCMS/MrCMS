using System;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models.UserSubscriptionReports
{

    public class UserRegistrationReportsSearchQuery
    {
        private DateTime _startDate;
        private DateTime _endDate;

        public UserRegistrationReportsSearchQuery()
        {
            _startDate = DateTime.MinValue;
            _endDate = DateTime.MinValue;
        }

        [Required(ErrorMessage = "*start date")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate
        {
            get
            {
                if (_startDate == DateTime.MinValue)
                {
                    _startDate = DateTime.UtcNow.AddDays(-90);
                    return _startDate.Date;
                }
                else
                    return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }
        [Required(ErrorMessage = "*end date")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate
        {
            get
            {
                if (_endDate == DateTime.MinValue)
                {
                    _endDate = DateTime.UtcNow;
                    return _endDate.Date;
                }
                else
                    return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }
    }
}