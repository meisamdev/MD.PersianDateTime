using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMember.Local

namespace MD.PersianDateTime.Core;

public partial struct PersianDateTime
{

#pragma warning disable IDE0079 // Remove unnecessary suppression
    [SuppressMessage("ReSharper", "InconsistentNaming")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
    private enum AmPmEnum
    {
        AM = 0,
        PM = 1,
        None = 2
    }
}