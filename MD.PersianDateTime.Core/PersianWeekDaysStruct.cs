using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMember.Local

namespace MD.PersianDateTime.Core;

public partial struct PersianDateTime
{
#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    private struct PersianWeekDaysStruct
    {
        public static KeyValuePair<int, string> شنبه => new((int)DayOfWeek.Saturday, "شنبه");
        public static KeyValuePair<int, string> یکشنبه => new((int)DayOfWeek.Sunday, "یکشنبه");
        public static KeyValuePair<int, string> دوشنبه => new((int)DayOfWeek.Monday, "دوشنبه");
        public static KeyValuePair<int, string> سه_شنبه => new((int)DayOfWeek.Tuesday, "سه شنبه");
        public static KeyValuePair<int, string> چهارشنبه => new((int)DayOfWeek.Thursday, "چهارشنبه");
        public static KeyValuePair<int, string> پنجشنبه => new((int)DayOfWeek.Wednesday, "پنج شنبه");
        public static KeyValuePair<int, string> جمعه => new((int)DayOfWeek.Friday, "جمعه");
    }
}