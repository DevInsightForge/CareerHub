namespace CareerHub.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly TimeZoneInfo TimeZoneBangladesh = FindTimeZone("Asia/Dhaka");

        private static TimeZoneInfo FindTimeZone(string timeZoneId)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new Exception($"Time zone '{timeZoneId}' not found.");
            }
        }

        public static DateTime ToBangladeshTime(this DateTime value)
        {
            DateTimeOffset dateTimeOffsetValue = new(value, TimeSpan.Zero);
            DateTimeOffset convertedOffset = TimeZoneInfo.ConvertTime(dateTimeOffsetValue, TimeZoneBangladesh);
            return convertedOffset.DateTime.ToUniversalTime();
        }
    }
}
