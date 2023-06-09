using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMember.Local

namespace MD.PersianDateTime.Core;

public partial struct PersianDateTime
{
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    private enum PersianDateTimeMonthEnum
    {
        فروردین = 1,
        اردیبهشت = 2,
        خرداد = 3,
        تیر = 4,
        مرداد = 5,
        شهریور = 6,
        مهر = 7,
        آبان = 8,
        آذر = 9,
        دی = 10,
        بهمن = 11,
        اسفند = 12
    }
}