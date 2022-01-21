using System.ComponentModel.DataAnnotations;

namespace MrCMS.Settings
{
    public enum GoogleRecaptchaKeyType
    {
        [Display(Name = "score-based (no challenge)")]
        ScoreBased,
        [Display(Name = "checkbox (checkbox challenge)")]
        Checkbox
    }
}