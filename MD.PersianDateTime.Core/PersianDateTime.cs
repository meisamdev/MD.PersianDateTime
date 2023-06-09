using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using MD.PersianDateTime.Core.Helpers;

// ReSharper disable UnusedMember.Local

namespace MD.PersianDateTime.Core;

/// <summary>
/// Created By Mohammad Dayyan, @mdssoft
/// 1397/01/10
/// </summary>
[Serializable]
public partial struct PersianDateTime :
    ISerializable, IFormattable, IConvertible,
    IComparable<PersianDateTime>, IComparable<DateTime>,
    IEquatable<PersianDateTime>, IEquatable<DateTime>
{

    #region ctor
    /// <summary>
    /// متد سازنده برای دی سریالایز شدن
    /// </summary>
    private PersianDateTime(SerializationInfo info, StreamingContext context) : this()
    {
        _dateTime = info.GetDateTime("DateTime");
        PersianNumber = info.GetBoolean("PersianNumber");
    }

    /// <summary>
    /// مقدار دهی اولیه با استفاده از دیت تایم میلادی
    /// </summary>
    /// <param name="dateTime">DateTime</param>
    /// <param name="persianNumber">آیا اعداد در خروجی های این آبجکت به صورت فارسی نمایش داده شوند یا فارسی؟</param>
    private PersianDateTime(DateTime? dateTime, bool persianNumber) : this()
    {
        _dateTime = dateTime ?? DateTime.MinValue;
        PersianNumber = persianNumber;
    }

    /// <summary>
    /// مقدار دهی اولیه با استفاده از دیت تایم میلادی
    /// </summary>
    public PersianDateTime(DateTime? nullableDateTime) : this()
    {
        _dateTime = nullableDateTime ?? DateTime.MinValue;
    }

    /// <summary>
    /// مقدار دهی اولیه با استفاده از سال، ماه و روز تاریخ شمسی
    /// </summary>
    /// <param name="year">سال شمسی</param>
    /// <param name="month">ماه شمسی</param>
    /// <param name="day">روز شمسی</param>
    public PersianDateTime(int year, int month, int day) : this(year, month, day, 0, 0, 0) { }

    /// <summary>
    /// مقدار دهی اولیه
    /// </summary>
    /// <param name="year">سال شمسی</param>
    /// <param name="month">ماه شمسی</param>
    /// <param name="day">روز شمسی</param>
    /// <param name="hour">ساعت</param>
    /// <param name="minute">دقیقه</param>
    /// <param name="second">ثانیه</param>
    public PersianDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, 0) { }

    /// <summary>
    /// مقدار دهی اولیه
    /// </summary>
    /// <param name="year">سال شمسی</param>
    /// <param name="month">ماه شمسی</param>
    /// <param name="day">روز شمسی</param>
    /// <param name="hour">سال</param>
    /// <param name="minute">دقیقه</param>
    /// <param name="second">ثانیه</param>
    /// <param name="milliseconds">میلی ثانیه</param>
    public PersianDateTime(int year, int month, int day, int hour, int minute, int second, int milliseconds) : this()
    {
        _dateTime = PersianCalendar.ToDateTime(year, month, day, hour, minute, second, milliseconds);
    }

    #endregion

    #region properties and fields

    private static PersianCalendar _persianCalendar;
    private static PersianCalendar PersianCalendar
    {
        get
        {
            _persianCalendar ??= new();

            return _persianCalendar;
        }
    }
    private readonly DateTime _dateTime;

    /// <summary>
    /// آیا اعداد در خروجی به صورت انگلیسی نمایش داده شوند؟
    /// </summary>
    public bool PersianNumber { get; set; }

    private static ReadOnlySpan<string> GregorianWeekDayNames => new[] { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
    private static ReadOnlySpan<string> GregorianMonthNames => new[] { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
    private static ReadOnlySpan<string> PmAm => new[] { "pm", "am" };


    /// <summary>
    /// سال شمسی
    /// </summary>
    public readonly int Year => _dateTime <= DateTime.MinValue ? DateTime.MinValue.Year : PersianCalendar.GetYear(_dateTime);

    /// <summary>
    /// ماه شمسی
    /// </summary>
    public readonly int Month => _dateTime <= DateTime.MinValue ? DateTime.MinValue.Month : PersianCalendar.GetMonth(_dateTime);

    /// <summary>
    /// نام فارسی ماه
    /// </summary>
    public readonly string MonthName => ((PersianDateTimeMonthEnum)Month).ToString();

    /// <summary>
    /// روز ماه
    /// </summary>
    public readonly int Day => _dateTime <= DateTime.MinValue ? DateTime.MinValue.Day : PersianCalendar.GetDayOfMonth(_dateTime);

    /// <summary>
    /// روز هفته
    /// </summary>
    public readonly DayOfWeek DayOfWeek => _dateTime <= DateTime.MinValue ? DateTime.MinValue.DayOfWeek : PersianCalendar.GetDayOfWeek(_dateTime);

    /// <summary>
    /// روز هفته یا ایندکس شمسی
    /// <para />
    /// شنبه دارای ایندکس صفر است
    /// </summary>
    public readonly PersianDayOfWeek PersianDayOfWeek => PersianCalendar.GetDayOfWeek(_dateTime) switch
    {
        DayOfWeek.Sunday => PersianDayOfWeek.Sunday,
        DayOfWeek.Monday => PersianDayOfWeek.Monday,
        DayOfWeek.Tuesday => PersianDayOfWeek.Tuesday,
        DayOfWeek.Wednesday => PersianDayOfWeek.Wednesday,
        DayOfWeek.Thursday => PersianDayOfWeek.Thursday,
        DayOfWeek.Friday => PersianDayOfWeek.Friday,
        DayOfWeek.Saturday => PersianDayOfWeek.Saturday,
        _ => throw new ArgumentOutOfRangeException(),
    };

    /// <summary>
    /// ساعت
    /// </summary>
    public readonly int Hour => _dateTime <= DateTime.MinValue ? 12 : PersianCalendar.GetHour(_dateTime);

    /// <summary>
    /// ساعت دو رقمی
    /// </summary>
    public readonly int ShortHour => Hour > 12 ? Hour - 12 : Hour;

    /// <summary>
    /// دقیقه
    /// </summary>
    public readonly int Minute => _dateTime <= DateTime.MinValue ? 0 : PersianCalendar.GetMinute(_dateTime);

    /// <summary>
    /// ثانیه
    /// </summary>
    public readonly int Second => _dateTime <= DateTime.MinValue ? 0 : PersianCalendar.GetSecond(_dateTime);

    /// <summary>
    /// میلی ثانیه
    /// </summary>
    public readonly int Millisecond => _dateTime <= DateTime.MinValue ? 0 : (int)PersianCalendar.GetMilliseconds(_dateTime);

    /// <summary>
    /// تعداد روز در ماه
    /// </summary>
    public readonly int GetMonthDays => Month switch
    {
        >= 1 and <= 6 => 31,
        >= 7 and <= 11 => 30,
        12 when IsLeapYear => 30,
        12 => 29,
        _ => throw new Exception("Month number is wrong !!!"),
    };

    /// <summary>
    /// هفته چندم سال
    /// </summary>
    public readonly int GetWeekOfYear => _dateTime <= DateTime.MinValue ? 0 : PersianCalendar.GetWeekOfYear(_dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Saturday);

    /// <summary>
    /// هفته چندم ماه
    /// </summary>
    public int GetWeekOfMonth => _dateTime <= DateTime.MinValue ? 0 : AddDays(1 - Day).GetWeekOfYear + 1;

    /// <summary>
    /// روز چندم سال
    /// </summary>
    public readonly int GetDayOfYear => _dateTime <= DateTime.MinValue ? 0 : PersianCalendar.GetDayOfYear(_dateTime);

    /// <summary>
    /// آیا سال کبیسه است؟
    /// </summary>
    public readonly bool IsLeapYear => _dateTime <= DateTime.MinValue ? false : PersianCalendar.IsLeapYear(Year);

    /// <summary>
    /// قبل از ظهر، بعد از ظهر
    /// </summary>
    private readonly AmPmEnum PersianAmPm => _dateTime.ToString("tt") == "PM" ? AmPmEnum.PM : AmPmEnum.AM;

    /// <summary>
    /// قبل از ظهر، بعد از ظهر به شکل مخفف . کوتاه
    /// </summary>
    public readonly string GetPersianAmPm => PersianAmPm switch
    {
        AmPmEnum.AM => "ق.ظ",
        AmPmEnum.PM => "ب.ظ",
        _ => "",
    };

    /// <summary>
    /// نام کامل ماه
    /// </summary>
    public readonly string GetLongMonthName => GetPersianMonthNamePrivate(Month);

    /// <summary>
    /// سال دو رقمی
    /// </summary>
    public readonly int GetShortYear => Year % 100;

    /// <summary>
    /// نام کامل روز
    /// </summary>
    public readonly string GetLongDayOfWeekName => DayOfWeek switch
    {
        DayOfWeek.Saturday => PersianWeekDaysStruct.شنبه.Value,
        DayOfWeek.Sunday => PersianWeekDaysStruct.یکشنبه.Value,
        DayOfWeek.Monday => PersianWeekDaysStruct.دوشنبه.Value,
        DayOfWeek.Tuesday => PersianWeekDaysStruct.سه_شنبه.Value,
        DayOfWeek.Wednesday => PersianWeekDaysStruct.چهارشنبه.Value,
        DayOfWeek.Thursday => PersianWeekDaysStruct.پنجشنبه.Value,
        DayOfWeek.Friday => PersianWeekDaysStruct.جمعه.Value,
        _ => throw new ArgumentOutOfRangeException(),
    };

    /// <summary>
    /// نام یک حرفی روز، حرف اول روز
    /// </summary>
    public string GetShortDayOfWeekName => DayOfWeek switch
    {
        DayOfWeek.Saturday => "ش",
        DayOfWeek.Sunday => "ی",
        DayOfWeek.Monday => "د",
        DayOfWeek.Tuesday => "س",
        DayOfWeek.Wednesday => "چ",
        DayOfWeek.Thursday => "پ",
        DayOfWeek.Friday => "ج",
        _ => throw new ArgumentOutOfRangeException(),
    };

    /// <summary>
    /// تاریخ و زمان همین الان
    /// </summary>
    public static PersianDateTime Now => new(DateTime.Now);

    /// <summary>
    /// تاریخ امروز
    /// </summary>
    public static PersianDateTime Today => new(DateTime.Today);

    /// <summary>
    /// زمان به فرمتی مشابه
    /// <para />
    /// 13:47:40:530
    /// </summary>
    public readonly string TimeOfDay => PersianNumber ? ToPersianNumber($"{Hour:00}:{Minute:00}:{Second:00}:{Millisecond:000}") : $"{Hour:00}:{Minute:00}:{Second:00}:{Millisecond:000}";

    /// <summary>
    /// زمان به فرمتی مشابه زیر
    /// <para />
    /// ساعت 01:47:40:530 ب.ظ
    /// </summary>
    public readonly string LongTimeOfDay => PersianNumber ? ToPersianNumber($"ساعت {ShortHour:00}:{Minute:00}:{Second:00}:{Millisecond:000} {GetPersianAmPm}") : $"ساعت {ShortHour:00}:{Minute:00}:{Second:00}:{Millisecond:000} {GetPersianAmPm}";

    /// <summary>
    /// زمان به فرمتی مشابه زیر
    /// <para />
    /// 01:47:40 ب.ظ
    /// </summary>
    public readonly string ShortTimeOfDay => PersianNumber ? ToPersianNumber($"{ShortHour:00}:{Minute:00}:{Second:00} {GetPersianAmPm}") : $"{ShortHour:00}:{Minute:00}:{Second:00} {GetPersianAmPm}";

    /// <summary>
    /// تاریخ بدون احتساب زمان
    /// </summary>
    public PersianDateTime Date => new(Year, Month, Day) { PersianNumber = PersianNumber };

    /// <summary>
    /// حداقل مقدار
    /// </summary>
    public static PersianDateTime MinValue => new(DateTime.MinValue);

    /// <summary>
    /// حداکثر مقدار
    /// </summary>
    public static PersianDateTime MaxValue => new(DateTime.MaxValue);

    #endregion

    #region override

    /// <summary>
    /// تبدیل تاریخ به رشته با فرمت مشابه زیر
    /// <para />
    /// 1393/09/14   13:49:40
    /// </summary>
    public override string ToString() => ToString(null, null);
    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is PersianDateTime time && _dateTime == time.ToDateTime();

    /// <inheritdoc />
    public override int GetHashCode() => _dateTime.GetHashCode();

    /// <summary>
    /// مقایسه با تاریخ دیگر
    /// </summary>
    /// <returns>مقدار بازگشتی همانند مقدار بازگشتی متد کامپیر در دیت تایم دات نت است</returns>
    public int CompareTo(PersianDateTime otherPersianDateTime) => _dateTime.CompareTo(otherPersianDateTime.ToDateTime());

    /// <summary>
    /// مقایسه با تاریخ دیگر
    /// </summary>
    /// <returns>مقدار بازگشتی همانند مقدار بازگشتی متد کامپیر در دیت تایم دات نت است</returns>
    public int CompareTo(DateTime otherDateTime) => _dateTime.CompareTo(otherDateTime);

    #region operators

    /// <summary>
    /// تبدیل خودکار به دیت تایم میلادی
    /// </summary>
    public static implicit operator DateTime(PersianDateTime persianDateTime)
    {
        return persianDateTime.ToDateTime();
    }

    /// <summary>
    /// اپراتور برابر
    /// </summary>
    public static bool operator ==(PersianDateTime persianDateTime1, PersianDateTime persianDateTime2)
    {
        return persianDateTime1.Equals(persianDateTime2);
    }

    /// <summary>
    /// اپراتور نامساوی
    /// </summary>
    public static bool operator !=(PersianDateTime persianDateTime1, PersianDateTime persianDateTime2)
    {
        return !persianDateTime1.Equals(persianDateTime2);
    }

    /// <summary>
    /// اپراتور بزرگتری
    /// </summary>
    public static bool operator >(PersianDateTime persianDateTime1, PersianDateTime persianDateTime2)
    {
        return persianDateTime1.ToDateTime() > persianDateTime2.ToDateTime();
    }

    /// <summary>
    /// اپراتور کوچکتری
    /// </summary>
    public static bool operator <(PersianDateTime persianDateTime1, PersianDateTime persianDateTime2)
    {
        return persianDateTime1.ToDateTime() < persianDateTime2.ToDateTime();
    }

    /// <summary>
    /// اپراتور بزرگتر مساوی
    /// </summary>
    public static bool operator >=(PersianDateTime persianDateTime1, PersianDateTime persianDateTime2)
    {
        return persianDateTime1.ToDateTime() >= persianDateTime2.ToDateTime();
    }

    /// <summary>
    /// اپراتور کوچکتر مساوی
    /// </summary>
    public static bool operator <=(PersianDateTime persianDateTime1, PersianDateTime persianDateTime2)
    {
        return persianDateTime1.ToDateTime() <= persianDateTime2.ToDateTime();
    }

    /// <summary>
    /// اپراتور جمع تو زمان
    /// </summary>
    public static PersianDateTime operator +(PersianDateTime persianDateTime1, TimeSpan timeSpanToAdd)
    {
        DateTime dateTime1 = persianDateTime1;
        return new PersianDateTime(dateTime1.Add(timeSpanToAdd));
    }

    /// <summary>
    /// اپراتور کم کردن دو زمان از هم
    /// </summary>
    public static TimeSpan operator -(PersianDateTime persianDateTime1, PersianDateTime persianDateTime2)
    {
        DateTime dateTime1 = persianDateTime1;
        DateTime dateTime2 = persianDateTime2;
        return dateTime1 - dateTime2;
    }

    #endregion

    #endregion

    #region ISerializable

    /// <inheritdoc />
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("DateTime", ToDateTime());
        info.AddValue("PersianNumber", PersianNumber);
    }

    #endregion

    #region IComparable

    /// <inheritdoc />
    public bool Equals(PersianDateTime other)
    {
        return Year == other.Year &&
            Month == other.Month &&
            Day == other.Day &&
            Hour == other.Hour &&
            Minute == other.Minute &&
            Second == other.Second &&
            Millisecond == other.Millisecond;
    }

    /// <inheritdoc />
    public bool Equals(DateTime other) => _dateTime == other;

    #endregion

    #region Methods

    /// <summary>
    /// تاریخ شروع ماه رمضان
    /// <para />
    /// چون ممکن است در یک سال شمسی دو شروع ماه رمضان داشته باشیم
    /// <para />
    /// مقدار بازگشتی آرایه است که حداکثر دو آیتم دارد
    /// </summary>
    public PersianDateTime[] GetStartDayOfRamadan(int hijriAdjustment)
    {
        HijriCalendar hijriCalendar = new() { HijriAdjustment = hijriAdjustment };

        var currentHijriYear = hijriCalendar.GetYear(_dateTime);

        PersianDateTime startDayOfRamadan1 = new(hijriCalendar.ToDateTime(currentHijriYear, 9, 1, 0, 0, 0, 0));

        PersianDateTime startDayOfRamadan2 = new(hijriCalendar.ToDateTime(++currentHijriYear, 9, 1, 0, 0, 0, 0));
        if (startDayOfRamadan1.Year == startDayOfRamadan2.Year)
            return new[] { startDayOfRamadan1, startDayOfRamadan2 };

        return new[] { startDayOfRamadan1 };
    }

    /// <summary>
    /// تاریخ آخرین روز ماه شمسی
    /// </summary>
    public PersianDateTime GetPersianDateOfLastDayOfMonth() => new PersianDateTime(Year, Month, GetMonthDays);
    /// <summary>
    /// تاریخ آخرین روز سال شمسی
    /// </summary>
    public PersianDateTime GetPersianDateOfLastDayOfYear() => new PersianDateTime(Year, 12, IsLeapYear ? 30 : 29);

    /// <summary>
    /// پارس کردن رشته و تبدیل به نوع PersianDateTime
    /// </summary>
    /// <param name="persianDateTimeInString">متنی که باید پارس شود</param>
    /// <param name="dateSeparatorPattern">کاراکتری که جدا کننده تاریخ ها است</param>
    public static PersianDateTime Parse(string persianDateTimeInString, string dateSeparatorPattern = @"\/|-")
    {
        //Convert persian and arabic digit to english to avoid throwing exception in Parse method
        persianDateTimeInString = ExtensionsHelper.ConvertDigitsToLatin(persianDateTimeInString);

        string month = "", year, day,
            hour = "0",
            minute = "0",
            second = "0",
            milliseconds = "0";
        var amPmEnum = AmPmEnum.None;
        var containMonthSeparator = Regex.IsMatch(persianDateTimeInString, dateSeparatorPattern);

        persianDateTimeInString = ToEnglishNumber(persianDateTimeInString.Replace("&nbsp;", " ").Replace(" ", "-").Replace("\\", "-"));
        persianDateTimeInString = Regex.Replace(persianDateTimeInString, dateSeparatorPattern, "-");
        persianDateTimeInString = persianDateTimeInString.Replace("ك", "ک").Replace("ي", "ی");

        persianDateTimeInString = $"-{persianDateTimeInString}-";

        // بدست آوردن ب.ظ یا ق.ظ
        if (persianDateTimeInString.IndexOf("ق.ظ", StringComparison.InvariantCultureIgnoreCase) > -1)
            amPmEnum = AmPmEnum.AM;
        else if (persianDateTimeInString.IndexOf("ب.ظ", StringComparison.InvariantCultureIgnoreCase) > -1)
            amPmEnum = AmPmEnum.PM;

        if (persianDateTimeInString.IndexOf(":", StringComparison.InvariantCultureIgnoreCase) > -1) // رشته ورودی شامل ساعت نیز هست
        {
            persianDateTimeInString = Regex.Replace(persianDateTimeInString, @"-*:-*", ":");
            hour = Regex.Match(persianDateTimeInString, @"(?<=-)\d{1,2}(?=:)", RegexOptions.IgnoreCase).Value;
            minute = Regex.Match(persianDateTimeInString, @"(?<=-\d{1,2}:)\d{1,2}(?=:?)", RegexOptions.IgnoreCase).Value;
            if (persianDateTimeInString.IndexOf(':') != persianDateTimeInString.LastIndexOf(':'))
            {
                second = Regex.Match(persianDateTimeInString, @"(?<=-\d{1,2}:\d{1,2}:)\d{1,2}(?=(\d{1,2})?)", RegexOptions.IgnoreCase).Value;
                milliseconds = Regex.Match(persianDateTimeInString, @"(?<=-\d{1,2}:\d{1,2}:\d{1,2}:)\d{1,4}(?=(\d{1,2})?)", RegexOptions.IgnoreCase).Value;
                if (string.IsNullOrEmpty(milliseconds)) milliseconds = "0";
            }
        }

        if (containMonthSeparator)
        {
            // بدست آوردن ماه
            month = Regex.Match(persianDateTimeInString, @"(?<=\d{2,4}-)\d{1,2}(?=-\d{1,2}-\d{1,2}(?!-\d{1,2}:))", RegexOptions.IgnoreCase).Value;
            if (string.IsNullOrEmpty(month))
                month = Regex.Match(persianDateTimeInString, @"(?<=\d{2,4}-)\d{1,2}(?=-\d{1,2}[^:])", RegexOptions.IgnoreCase).Value;

            // بدست آوردن روز
            day = Regex.Match(persianDateTimeInString, @"(?<=\d{2,4}-\d{1,2}-)\d{1,2}(?=-)", RegexOptions.IgnoreCase).Value;

            // بدست آوردن سال
            year = Regex.Match(persianDateTimeInString, @"(?<=-)\d{2,4}(?=-\d{1,2}-\d{1,2})", RegexOptions.IgnoreCase).Value;
        }
        else
        {
            foreach (PersianDateTimeMonthEnum item in Enum.GetValues(typeof(PersianDateTimeMonthEnum)))
            {
                var itemValueInString = item.ToString();
                if (!persianDateTimeInString.Contains(itemValueInString)) continue;
                month = ((int)item).ToString();
                break;
            }

            if (string.IsNullOrEmpty(month))
                throw new Exception("عدد یا حرف ماه در رشته ورودی وجود ندارد");

            // بدست آوردن روز
            var dayMatch = Regex.Match(persianDateTimeInString, @"(?<=-)\d{1,2}(?=-)", RegexOptions.IgnoreCase);
            if (dayMatch.Success)
            {
                day = dayMatch.Value;
                persianDateTimeInString = Regex.Replace(persianDateTimeInString, $"(?<=-){day}(?=-)", "");
            }
            else
                throw new Exception("عدد روز در رشته ورودی وجود ندارد");

            // بدست آوردن سال
            var yearMatch = Regex.Match(persianDateTimeInString, @"(?<=-)\d{4}(?=-)", RegexOptions.IgnoreCase);
            if (yearMatch.Success)
                year = yearMatch.Value;
            else
            {
                yearMatch = Regex.Match(persianDateTimeInString, @"(?<=-)\d{2,4}(?=-)", RegexOptions.IgnoreCase);
                if (yearMatch.Success)
                    year = yearMatch.Value;
                else
                    throw new Exception("عدد سال در رشته ورودی وجود ندارد");
            }
        }

        //if (year.Length <= 2 && year[0] == '9') year = string.Format("13{0}", year);
        //else if (year.Length <= 2) year = string.Format("14{0}", year);

        var numericYear = int.Parse(year);
        var numericMonth = int.Parse(month);
        var numericDay = int.Parse(day);
        var numericHour = int.Parse(hour);
        var numericMinute = int.Parse(minute);
        var numericSecond = int.Parse(second);
        var numericMillisecond = int.Parse(milliseconds);

        if (numericYear < 100)
            numericYear += 1300;

        switch (amPmEnum)
        {
            case AmPmEnum.PM:
                if (numericHour < 12)
                    numericHour = numericHour + 12;
                break;
            case AmPmEnum.AM:
            case AmPmEnum.None:
                break;
        }

        return new PersianDateTime(numericYear, numericMonth, numericDay, numericHour, numericMinute, numericSecond, numericMillisecond);
    }

    /// <summary>
    /// پارس کردن یک رشته برای یافتن تاریخ شمسی
    /// </summary>
    public static bool TryParse(string? persianDateTimeInString, out PersianDateTime result, string dateSeparatorPattern = @"\/|-")
    {
        if (string.IsNullOrEmpty(persianDateTimeInString))
        {
            result = MinValue;
            return false;
        }

        try
        {
            result = Parse(persianDateTimeInString, dateSeparatorPattern);
            return true;
        }
        catch
        {
            result = MinValue;
            return false;
        }
    }

    /// <summary>
    /// پارس کردن عددی در فرمت تاریخ شمسی
    /// <para />
    /// همانند 13920305
    /// </summary>
    public static PersianDateTime Parse(int numericPersianDate)
    {
        if (numericPersianDate.ToString().Length != 8)
            throw new InvalidCastException("Numeric persian date must have a format like 13920101.");

        var year = numericPersianDate / 10000;
        var day = numericPersianDate % 100;
        var month = numericPersianDate / 100 % 100;
        return new PersianDateTime(year, month, day);
    }
    /// <summary>
    /// پارس کردن عددی در فرمت تاریخ شمسی
    /// <para />
    /// همانند 13920305
    /// </summary>
    public static bool TryParse(int numericPersianDate, out PersianDateTime result)
    {
        try
        {
            result = Parse(numericPersianDate);
            return true;
        }
        catch
        {
            result = MinValue;
            return false;
        }
    }

    /// <summary>
    /// پارس کردن عددی در فرمت تاریخ و زمان شمسی
    /// <para />
    /// همانند 13961223072132004
    /// </summary>
    public static PersianDateTime Parse(long numericPersianDateTime)
    {
        if (numericPersianDateTime.ToString().Length != 17)
            throw new InvalidCastException("Numeric persian date time must have a format like 1396122310223246.");

        var year = (int)(numericPersianDateTime / 10000000000000);
        var month = (int)(numericPersianDateTime / 100000000000 % 100);
        var day = (int)numericPersianDateTime / 1000000000 % 100;
        var hour = (int)numericPersianDateTime / 10000000 % 100;
        var minute = (int)numericPersianDateTime / 100000 % 100;
        var second = (int)numericPersianDateTime / 1000 % 100;
        var millisecond = (int)numericPersianDateTime % 1000;

        return new PersianDateTime(year, month, day, hour, minute, second, millisecond);
    }

    /// <summary>
    /// پارس کردن عددی در فرمت تاریخ و زمان شمسی
    /// <para />
    /// همانند 13961223102232461
    /// </summary>
    public static bool TryParse(long numericPersianDateTime, out PersianDateTime result)
    {
        try
        {
            result = Parse(numericPersianDateTime);
            return true;
        }
        catch
        {
            result = MinValue;
            return false;
        }
    }

    /// <summary>
    /// تبدیل به ساعت گرینویچ
    /// </summary>
    public PersianDateTime ToUniversalTime() => new(_dateTime.ToUniversalTime());

    /// <summary>
    /// دریافت تعداد ثانیه های سپری شده از اولین روز سال 1397
    /// </summary>
    public int ToEpochTime()
    {
        var timeSpan = _dateTime - new DateTime(1970, 1, 1);
        return (int)timeSpan.TotalSeconds;
    }

    /// <summary>
    /// فرمت های که پشتیبانی می شوند
    /// <para />
    /// yyyy: سال چهار رقمی
    /// <para />
    /// yy: سال دو رقمی
    /// <para />
    /// MMMM: نام فارسی ماه
    /// <para />
    /// MM: عدد دو رقمی ماه
    /// <para />
    /// M: عدد یک رقمی ماه
    /// <para />
    /// dddd: نام فارسی روز هفته
    /// <para />
    /// dd: عدد دو رقمی روز ماه
    /// <para />
    /// d: عدد یک رقمی روز ماه
    /// <para />
    /// HH: ساعت دو رقمی با فرمت 00 تا 24
    /// <para />
    /// H: ساعت یک رقمی با فرمت 0 تا 24
    /// <para />
    /// hh: ساعت دو رقمی با فرمت 00 تا 12
    /// <para />
    /// h: ساعت یک رقمی با فرمت 0 تا 12
    /// <para />
    /// mm: عدد دو رقمی دقیقه
    /// <para />
    /// m: عدد یک رقمی دقیقه
    /// <para />
    /// ss: ثانیه دو رقمی
    /// <para />
    /// s: ثانیه یک رقمی
    /// <para />
    /// fff: میلی ثانیه 3 رقمی
    /// <para />
    /// ff: میلی ثانیه 2 رقمی
    /// <para />
    /// f: میلی ثانیه یک رقمی
    /// <para />
    /// tt: ب.ظ یا ق.ظ
    /// <para />
    /// t: حرف اول از ب.ظ یا ق.ظ
    /// </summary>
    public string ToString(string? format, IFormatProvider? provider = null)
    {
        format ??= "yyyy/MM/dd   HH:mm:ss";
        provider ??= CultureInfo.InvariantCulture;

        var dateTimeString = format.Replace("yyyy", Year.ToString(provider))
            .Replace("yy", GetShortYear.ToString("00", provider))
            .Replace("MMMM", MonthName)
            .Replace("MM", Month.ToString("00", provider))
            .Replace("M", Month.ToString(provider))
            .Replace("dddd", GetLongDayOfWeekName)
            .Replace("dd", Day.ToString("00", provider))
            .Replace("d", Day.ToString(provider))
            .Replace("HH", Hour.ToString("00", provider))
            .Replace("H", Hour.ToString(provider))
            .Replace("hh", ShortHour.ToString("00", provider))
            .Replace("h", ShortHour.ToString(provider))
            .Replace("mm", Minute.ToString("00", provider))
            .Replace("m", Minute.ToString(provider))
            .Replace("ss", Second.ToString("00", provider))
            .Replace("s", Second.ToString(provider))
            .Replace("fff", Millisecond.ToString("000", provider))
            .Replace("ff", (Millisecond / 10).ToString("00", provider))
            .Replace("f", (Millisecond / 100).ToString(provider))
            .Replace("tt", GetPersianAmPm)
            .Replace("t", GetPersianAmPm[0].ToString(provider));

        if (PersianNumber)
            return ToPersianNumber(dateTimeString.Trim());

        return dateTimeString.Trim();
    }

    /// <summary>
    /// بررسی میکند آیا تاریخ ورودی تاریخ میلادی است یا نه
    /// </summary>
    public static bool IsChristianDate(string inputString)
    {
        inputString = inputString.ToLower();
        bool result;

        foreach (var gregorianWeekDayName in GregorianWeekDayNames)
        {
            result = inputString.Contains(gregorianWeekDayName);
            if (result) return true;
        }

        foreach (var gregorianMonthName in GregorianMonthNames)
        {
            result = inputString.Contains(gregorianMonthName);
            if (result) return true;
        }

        foreach (var item in PmAm)
        {
            result = inputString.Contains(item);
            if (result) return true;
        }

        return Regex.IsMatch(inputString, @"(1[8-9]|[2-9][0-9])\d{2}", RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// بررسی میکند آیا تاریخ ورودی مطابق  تاریخ اس کسو ال سرور می باشد یا نه
    /// </summary>
    public static bool IsSqlDateTime(DateTime dateTime) => dateTime >= new DateTime(1753, 1, 1);
    /// <summary>
    /// تبدیل نام ماه شمسی به عدد معادل آن
    /// <para />
    /// به طور مثال آذر را به 9 تبدیل می کند
    /// </summary>
    public int GetMonthEnum(string longMonthName) => (int)(PersianDateTimeMonthEnum)Enum.Parse(typeof(PersianDateTimeMonthEnum), longMonthName);

    /// <summary>
    /// نمایش تاریخ به فرمتی مشابه زیر
    /// <para />
    /// 1393/09/14
    /// </summary>
    public string ToShortDateString() => PersianNumber ? ToPersianNumber($"{Year:0000}/{Month:00}/{Day:00}") : $"{Year:0000}/{Month:00}/{Day:00}";

    /// <summary>
    /// نمایش تاریخ به فرمتی مشابه زیر
    /// <para />
    /// ج 14 آذر 93
    /// </summary>
    public string ToShortDate1String() => PersianNumber ? ToPersianNumber($"{GetShortDayOfWeekName} {Day:00} {GetLongMonthName} {GetShortYear}") : $"{GetShortDayOfWeekName} {Day:00} {GetLongMonthName} {GetShortYear}";

    /// <summary>
    /// نمایش تاریخ به صورت عدد و در فرمتی مشابه زیر
    /// <para />
    /// 13930914
    /// </summary>
    public int ToShortDateInt() => int.Parse($"{Year:0000}{Month:00}{Day:00}");

    /// <summary>
    /// نمایش تاریخ و ساعت تا دقت میلی ثانیه به صورت عدد
    /// <para />
    /// 1396122310324655
    /// </summary>
    public long ToLongDateTimeInt() => long.Parse($"{Year:0000}{Month:00}{Day:00}{Hour:00}{Minute:00}{Second:00}{Millisecond:000}");

    /// <summary>
    /// در این فرمت نمایش ساعت و دقیقه و ثانیه در کنار هم با حذف علامت : تبدیل به عدد می شوند و نمایش داده می شود
    /// <para />
    /// مثال: 123452
    /// <para />
    /// که به معنای ساعت 12 و 34 دقیقه و 52 ثانیه می باشد
    /// </summary>
    public int ToTimeInt() => int.Parse($"{Hour:00}{Minute:00}{Second:00}");

    /// <summary>
    /// در این فرمت نمایش ساعت و دقیقه در کنار هم با حذف علامت : تبدیل به عدد می شوند و نمایش داده می شود
    /// <para />
    /// مثال: 1234
    /// <para />
    /// که به معنای ساعت 12 و 34 دقیقه می باشد
    /// </summary>
    public int ToTimeInt1() => int.Parse($"{Hour:00}{Minute:00}");

    /// <summary>
    /// نمایش تاریخ به فرمتی مشابه زیر
    /// <para />
    /// جمعه، 14 آذر 1393
    /// </summary>
    public string ToLongDateString() => PersianNumber ? ToPersianNumber($"{GetLongDayOfWeekName}، {Day:00} {GetLongMonthName} {Year:0000}") : $"{GetLongDayOfWeekName}، {Day:00} {GetLongMonthName} {Year:0000}";

    /// <summary>
    /// نمایش تاریخ و زمان به فرمتی مشابه زیر
    /// <para />
    /// جمعه، 14 آذر 1393 ساعت 13:50:27
    /// </summary>
    public string ToLongDateTimeString() => PersianNumber ? ToPersianNumber($"{GetLongDayOfWeekName}، {Day:00} {GetLongMonthName} {Year:0000} ساعت {Hour:00}:{Minute:00}:{Second:00}") : $"{GetLongDayOfWeekName}، {Day:00} {GetLongMonthName} {Year:0000} ساعت {Hour:00}:{Minute:00}:{Second:00}";

    /// <summary>
    /// نمایش تاریخ و زمان به فرمتی مشابه زیر
    /// <para />
    /// جمعه، 14 آذر 1393 13:50
    /// </summary>
    public string ToShortDateTimeString() => PersianNumber ? ToPersianNumber($"{GetLongDayOfWeekName}، {Day:00} {GetLongMonthName} {Year:0000} {Hour:00}:{Minute:00}") : $"{GetLongDayOfWeekName}، {Day:00} {GetLongMonthName} {Year:0000} {Hour:00}:{Minute:00}";

    /// <summary>
    /// نمایش زمان به فرمتی مشابه زیر
    /// <para />
    /// 01:50 ب.ظ
    /// </summary>
    public string ToShortTimeString() => PersianNumber ? ToPersianNumber($"{ShortHour:00}:{Minute:00} {GetPersianAmPm}") : $"{ShortHour:00}:{Minute:00} {GetPersianAmPm}";

    /// <summary>
    /// نمایش زمان به فرمتی مشابه زیر
    /// <para />
    /// 13:50:20
    /// </summary>
    public string ToLongTimeString() => PersianNumber ? ToPersianNumber($"{Hour:00}:{Minute:00}:{Second:00}") : $"{Hour:00}:{Minute:00}:{Second:00}";

    /// <summary>
    /// نمایش زمان به فرمتی مشابه زیر
    /// <para />
    /// 1 دقیقه قبل
    /// </summary>
    public string ElapsedTime()
    {
        PersianDateTime persianDateTimeNow = new(DateTime.Now);
        var timeSpan = persianDateTimeNow - _dateTime;
        if (timeSpan.TotalDays > 90)
            return ToShortDateTimeString();

        var result = string.Empty;
        if (timeSpan.TotalDays > 30)
        {
            var month = timeSpan.TotalDays / 30;
            if (month > 0)
                result = $"{month:0} ماه قبل";
        }
        else if (timeSpan.TotalDays >= 1)
        {
            result = $"{timeSpan.TotalDays:0} روز قبل";
        }
        else if (timeSpan.TotalHours >= 1)
        {
            result = $"{timeSpan.TotalHours:0} ساعت قبل";
        }
        else
        {
            var minute = timeSpan.TotalMinutes;
            if (minute <= 1) minute = 1;
            result = $"{minute:0} دقیقه قبل";
        }

        if (PersianNumber)
            return ToPersianNumber(result);

        return result;
    }

    /// <summary>
    /// گرفتن فقط زمان
    /// </summary>
    public TimeSpan GetTime() => new(0, _dateTime.Hour, _dateTime.Minute, _dateTime.Second, _dateTime.Millisecond);

    /// <summary>
    /// تنظیم کردن زمان
    /// </summary>
    public PersianDateTime SetTime(int hour, int minute, int second = 0, int millisecond = 0) => new(Year, Month, Day, hour, minute, second, millisecond);

    /// <summary>
    /// تبدیل به تاریخ میلادی
    /// </summary>
    public DateTime ToDateTime() => _dateTime;

    /// <summary>
    /// تبدیل به تاریخ هجری قمری
    /// </summary>
    public HijriDateTime ToHijri(int hijriAdjustment = 0)
    {
        var hijriCalendar = new HijriCalendar { HijriAdjustment = hijriAdjustment };
        var month = hijriCalendar.GetMonth(_dateTime);
        string monthName = month switch
        {
            1 => "محرم",
            2 => "صفر",
            3 => "ربیع الاول",
            4 => "ربیع الثانی",
            5 => "جمادل الاولی",
            6 => "جمادی الاخره",
            7 => "رجب",
            8 => "شعبان",
            9 => "رمضان",
            10 => "شوال",
            11 => "ذوالقعده",
            12 => "ذوالحجه",
            _ => "",
        };
        return new HijriDateTime
        {
            Day = hijriCalendar.GetDayOfMonth(_dateTime),
            Month = month,
            Year = hijriCalendar.GetYear(_dateTime),
            MonthName = monthName
        };
    }

    /// <summary>
    /// کم کردن دو تاریخ از هم
    /// </summary>
    public TimeSpan Subtract(PersianDateTime persianDateTime) => _dateTime - persianDateTime.ToDateTime();

    /// <summary>
    /// تعداد ماه اختلافی با تاریخ دیگری را بر میگرداند
    /// </summary>
    /// <returns>تعداد ماه</returns>
    public int MonthDifference(DateTime dateTime) => Math.Abs(dateTime.Month - _dateTime.Month + 12 * (dateTime.Year - _dateTime.Year));

    /// <summary>
    /// اضافه کردن مدت زمانی به تاریخ
    /// </summary>
    public PersianDateTime Add(TimeSpan timeSpan) => new(_dateTime.Add(timeSpan), PersianNumber);

    /// <summary>
    /// اضافه کردن سال به تاریخ
    /// </summary>
    public PersianDateTime AddYears(int years) => new(PersianCalendar.AddYears(_dateTime, years), PersianNumber);

    /// <summary>
    /// اضافه کردن روز به تاریخ
    /// </summary>
    public PersianDateTime AddDays(int days) => new(PersianCalendar.AddDays(_dateTime, days), PersianNumber);

    /// <summary>
    /// اضافه کردن ماه به تاریخ
    /// </summary>
    public PersianDateTime AddMonths(int months) => new(PersianCalendar.AddMonths(_dateTime, months), PersianNumber);

    /// <summary>
    /// اضافه کردن ساعت به تاریخ
    /// </summary>
    public PersianDateTime AddHours(int hours) => new(_dateTime.AddHours(hours), PersianNumber);

    /// <summary>
    /// اضافه کردن دقیقه به تاریخ
    /// </summary>
    public PersianDateTime AddMinutes(int minutes) => new(_dateTime.AddMinutes(minutes), PersianNumber);

    /// <summary>
    /// اضافه کردن ثانیه به تاریخ
    /// </summary>
    public PersianDateTime AddSeconds(int seconds) => new(_dateTime.AddSeconds(seconds), PersianNumber);

    /// <summary>
    /// اضافه کردن میلی ثانیه به تاریخ
    /// </summary>
    public PersianDateTime AddMilliseconds(int milliseconds) => new(_dateTime.AddMilliseconds(milliseconds), PersianNumber);

    /// <summary>
    /// بدست آوردن تاریخ شمسی اولین روز هفته
    /// </summary>
    public PersianDateTime GetFirstDayOfWeek()
    {
        var persianDateTime = new PersianDateTime(_dateTime).Date;
        return persianDateTime.AddDays(PersianDayOfWeek.Saturday - persianDateTime.PersianDayOfWeek);
    }

    /// <summary>
    /// بدست آوردن تاریخ شمسی آخرین روز هفته
    /// </summary>
    public PersianDateTime GetPersianWeekend()
    {
        var persianDateTime = new PersianDateTime(_dateTime).Date;
        return persianDateTime.AddDays(PersianDayOfWeek.Friday - persianDateTime.PersianDayOfWeek);
    }

    /// <summary>
    /// نام فارسی ماه بر اساس شماره ماه
    /// </summary>
    /// <returns>نام فارسی ماه</returns>
    public static string GetPersianMonthName(int monthNumber) => GetPersianMonthNamePrivate(monthNumber);

    private static string? ToPersianNumber(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;

        input = input.Replace("ي", "ی").Replace("ك", "ک");

        //۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹
        return input.Replace("0", "۰")
                .Replace("1", "۱")
                .Replace("2", "۲")
                .Replace("3", "۳")
                .Replace("4", "۴")
                .Replace("5", "۵")
                .Replace("6", "۶")
                .Replace("7", "۷")
                .Replace("8", "۸")
                .Replace("9", "۹");
    }

    private static string ToEnglishNumber(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;

        input = input.Replace("ي", "ی").Replace("ك", "ک");

        //۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹
        return input.Replace(",", "")
            .Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9");
    }

    private static string GetPersianMonthNamePrivate(int monthNumber) => monthNumber switch
    {
        1 => "فروردین",
        2 => "اردیبهشت",
        3 => "خرداد",
        4 => "تیر",
        5 => "مرداد",
        6 => "شهریور",
        7 => "مهر",
        8 => "آبان",
        9 => "آذر",
        10 => "دی",
        11 => "بهمن",
        12 => "اسفند",
        _ => "",
    };

    #endregion

    #region GetDifferentMonths Base On Seasons

    /// <summary>
    /// ماه های مرتبط با یک فصل را برمی گرداند
    /// </summary>
    private static int[] GetSeasonMonths(int month)
    {
        return month switch
        {
            1 or 2 or 3 => new[] { 1, 2, 3 },
            4 or 5 or 6 => new[] { 4, 5, 6 },
            7 or 8 or 9 => new[] { 7, 8, 9 },
            10 or 11 or 12 => new[] { 10, 11, 12 },
            _ => throw new Exception($"month {month} is not valid to GetFirstQuarterMonths"),
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="firstSeasonMonths"></param>
    /// <returns></returns>
    private static Dictionary<int, int[]> GetSeasons(int[] firstSeasonMonths)
    {
        var quarter = 1;
        var quarterDictionary = new Dictionary<int, int[]> { { quarter, firstSeasonMonths } };
        var li = firstSeasonMonths;
        var dd = firstSeasonMonths[0];
        while (dd != 10)
        {
            var temp = firstSeasonMonths.Select(du => du + 3).ToArray();
            quarter += 1;
            quarterDictionary.Add(quarter, temp);
            firstSeasonMonths = temp;
            dd = firstSeasonMonths[0];
        }
        dd = 1;
        while (dd != li[0])
        {
            var temp = new[] { dd, dd + 1, dd + 2 };
            quarter += 1;
            quarterDictionary.Add(quarter, temp);
            firstSeasonMonths = temp;
            dd = firstSeasonMonths.Last() + 1;
        }

        return quarterDictionary;
    }

    /// <summary>
    /// اختلاف بین دو تاریخ بر حسب فصول
    /// </summary>
    public int GetDifferenceQuarter(DateTime targetDateTime)
    {
        var biggerDateTime = targetDateTime;
        var lesserDateTime = _dateTime;
        if (_dateTime > targetDateTime)
        {
            var tmp = biggerDateTime;
            biggerDateTime = _dateTime;
            lesserDateTime = tmp;
        }
        var diffMonth = (biggerDateTime.Year - lesserDateTime.Year) * 12 + biggerDateTime.Month - lesserDateTime.Month;
        var firstQuarter = GetSeasonMonths(lesserDateTime.Month);
        var seasons = GetSeasons(firstQuarter);
        if (diffMonth < 12) return seasons.First(p => p.Value.Contains(biggerDateTime.Month)).Key;
        var diffYear = diffMonth / 12;
        var diffQuarter = diffMonth - diffYear * 12;
        return diffYear * 4 + diffQuarter / 3 + 1;
    }

    #endregion

    #region IConvertible

    /// <inheritdoc />
    public TypeCode GetTypeCode() => TypeCode.DateTime;

    /// <inheritdoc />
    public bool ToBoolean(IFormatProvider provider) => _dateTime > DateTime.MinValue;

    /// <inheritdoc />
    public byte ToByte(IFormatProvider provider) => throw new InvalidCastException();

    /// <inheritdoc />
    public char ToChar(IFormatProvider provider) => throw new InvalidCastException();

    /// <inheritdoc />
    public DateTime ToDateTime(IFormatProvider provider) => _dateTime;

    /// <inheritdoc />
    public decimal ToDecimal(IFormatProvider provider) => ToLongDateTimeInt();

    /// <inheritdoc />
    public double ToDouble(IFormatProvider provider) => ToLongDateTimeInt();

    /// <inheritdoc />
    public short ToInt16(IFormatProvider provider) => throw new InvalidCastException();

    /// <inheritdoc />
    public int ToInt32(IFormatProvider provider) => ToShortDateInt();

    /// <inheritdoc />
    public long ToInt64(IFormatProvider provider) => ToLongDateTimeInt();

    /// <inheritdoc />
    public sbyte ToSByte(IFormatProvider provider) => throw new InvalidCastException();

    /// <inheritdoc />
    public float ToSingle(IFormatProvider provider) => throw new InvalidCastException();

    /// <inheritdoc />
    public string ToString(IFormatProvider provider) => ToString("", provider);

    /// <inheritdoc />
    public ushort ToUInt16(IFormatProvider provider) => throw new InvalidCastException();

    /// <inheritdoc />
    public uint ToUInt32(IFormatProvider provider) => (uint)ToShortDateInt();

    /// <inheritdoc />
    public ulong ToUInt64(IFormatProvider provider) => (ulong)ToLongDateTimeInt();

    /// <inheritdoc />
    public object ToType(Type conversionType, IFormatProvider provider) => Type.GetTypeCode(conversionType) switch
    {
        TypeCode.Boolean => ToBoolean(provider),
        TypeCode.Byte => ToByte(provider),
        TypeCode.Char => ToChar(provider),
        TypeCode.DateTime => ToDateTime(provider),
        TypeCode.Decimal => ToDecimal(provider),
        TypeCode.Double => ToDouble(provider),
        TypeCode.Int16 => ToInt16(provider),
        TypeCode.Int32 => ToInt32(provider),
        TypeCode.Int64 => ToInt64(provider),
        TypeCode.Object when typeof(PersianDateTime) == conversionType => this,
        TypeCode.Object when (typeof(DateTime) == conversionType) => ToDateTime(),
        TypeCode.SByte => ToSByte(provider),
        TypeCode.Single => ToSingle(provider),
        TypeCode.String => ToString(provider),
        TypeCode.UInt16 => ToUInt16(provider),
        TypeCode.UInt32 => ToUInt32(provider),
        TypeCode.UInt64 => ToUInt64(provider),
        //TypeCode.DBNull => throw new InvalidCastException(),
        //TypeCode.Empty => throw new InvalidCastException(),
        //TypeCode.Object => throw new InvalidCastException($"Conversion to a {conversionType.Name} is not supported."),
        _ => throw new InvalidCastException($"Conversion to {conversionType.Name} is not supported."),
    };

    #endregion
}