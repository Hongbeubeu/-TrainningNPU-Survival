using System;
using Npu.Core;

namespace Npu.Helper
{

    public static class TimeUtils
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        public static readonly DateTime EpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTicks => DateTime.Now.Ticks - Epoch.Ticks;

        public static long CurrentTicksUtc => DateTime.UtcNow.Ticks - EpochUtc.Ticks;

        public static double CurrentSeconds => TicksToSeconds(CurrentTicks);

        public static double CurrentSecondsUtc => TicksToSeconds(CurrentTicksUtc);

        public static DateTime TicksToDateTime(long ticks) => Epoch + TimeSpan.FromTicks(ticks);

        public static DateTime SecondsToDateTime(double secons) => Epoch + TimeSpan.FromSeconds(secons);

        public static double TimespanSeconds(long ticks, long lastTicks)
        {
            return TimeSpan.FromTicks(ticks - lastTicks).TotalSeconds;
        }

        public static double TimespanHours(long ticks, long lastTicks)
        {
            return TimeSpan.FromTicks(ticks - lastTicks).TotalHours;
        }

        public static double TimespanDays(long ticks, long lastTicks)
        {
            return TimeSpan.FromTicks(ticks - lastTicks).TotalDays;
        }

        public static double Timestamp(this DateTime dateTime)
        {
            return (dateTime - (dateTime.Kind == DateTimeKind.Local ? Epoch : EpochUtc)).TotalSeconds;
        }

        public static long SecondsToTicks(double seconds)
        {
            return (long) (seconds * TimeSpan.TicksPerSecond);
        }

        public static double TicksToSeconds(long ticks)
        {
            return (double) ticks / TimeSpan.TicksPerSecond;
        }

        public static string FormatTimeSpan(double seconds)
        {
            var span = new TimeSpan(SecondsToTicks(seconds));
            return span.Hours > 0
                ? $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}"
                : $"{span.Minutes:00}:{span.Seconds:00}";
        }

        public static string FormatFullSimple(double seconds)
        {
            var span = TimeSpan.FromSeconds(seconds);

            return span.Days > 0
                ? $"{span.Days}d {span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}"
                : $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
        }

        public static string FormatFullFromEpoch(double seconds)
        {
            var span = TimeSpan.FromSeconds(seconds);
            var time = new DateTime(span.Ticks);

            return time.ToString("HH:mm:ss dd/MM/yyyy");
        }

        public static long DateTimeToTicks(DateTime d) => d.Ticks - Epoch.Ticks;

        public static double DayOffset(this DateTime dateTime) => dateTime.TimeOfDay.TotalSeconds;
        public static double HourOffset(this DateTime dateTime) => dateTime.TimeOfDay.TotalSeconds % 3600;
        public static double MinuteOffset(this DateTime dateTime) => dateTime.TimeOfDay.TotalSeconds % 60;

        public static SecuredDouble TillMidnightTime => (86400 - DateTime.Now.DayOffset());

    }

}