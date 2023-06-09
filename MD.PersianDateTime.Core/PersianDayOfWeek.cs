using System.ComponentModel.DataAnnotations;

namespace MD.PersianDateTime.Core;

/// <summary>
/// روز های هفته
/// </summary>
public enum PersianDayOfWeek
{
    /// <summary>
    /// شنبه
    /// </summary>
    [Display(Name = "شنبه")]
    Saturday = 0,

    /// <summary>
    /// یک‌شنبه
    /// </summary>
    [Display(Name = "یک‌شنبه")]
    Sunday = 1,

    /// <summary>
    /// دوشنبه
    /// </summary>
    [Display(Name = "دوشنبه")]
    Monday = 2,

    /// <summary>
    /// سه‌شنبه
    /// </summary>
    [Display(Name = "سه‌شنبه")]
    Tuesday = 3,

    /// <summary>
    /// چهارشنبه
    /// </summary>
    [Display(Name = "چهارشنبه")]
    Wednesday = 4,

    /// <summary>
    /// پنج‌شنبه
    /// </summary>
    [Display(Name = "پنج‌شنبه")]
    Thursday = 5,

    /// <summary>
    /// جمعه
    /// </summary>
    [Display(Name = "جمعه")]
    Friday = 6
}
