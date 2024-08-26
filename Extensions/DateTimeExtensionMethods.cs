
namespace TradingBots.Extensions
{
    internal static class DateTimeExtensionMethods
    {
        /// <summary>
        /// Gets a date time normalised to minutes
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime NormaliseToMinute(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }
        
    }
}
