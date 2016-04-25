using System;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public struct BatchCompletionStatus
    {
        public int Total { get; set; }

        public int Pending { get; set; }
        public int Failed { get; set; }
        public int Succeeded { get; set; }

        public string AverageTimeTaken { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public TimeSpan EstimatedTimeRemaining { get; set; }

        public string PercentageCompleted
        {
            get
            {
                if (Total == 0)
                    return "100%";

                return (((decimal)Completed / (decimal)Total) * 100).ToString("0.00") + "%";
            }
        }

        public int Completed
        {
            get { return Failed + Succeeded; }
        }

        public string FullStatus
        {
            get { return string.Format("{0} ({1} of {2})", PercentageCompleted, Completed, Total); }
        }

    }
}