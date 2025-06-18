namespace DPTS.Applications.Shareds
{
    public class SharedHandle
    {
        public static (DateTime Start, DateTime End) GetWeekRange(int offsetWeek = 0)
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-diff).AddDays(7 * offsetWeek);
            var endOfWeek = startOfWeek.AddDays(6);
            return (startOfWeek, endOfWeek);
        }
        public static (DateTime Start, DateTime End) GetMonthRange(int offsetMonth = 0)
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1).AddMonths(offsetMonth);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return (firstDayOfMonth, lastDayOfMonth);
        }
        public static (DateTime Start, DateTime End) GetYearRange(int offsetYear = 0)
        {
            var today = DateTime.Today;
            var firstDayOfYear = new DateTime(today.Year + offsetYear, 1, 1);
            var lastDayOfYear = new DateTime(today.Year + offsetYear, 12, 31);
            return (firstDayOfYear, lastDayOfYear);
        }
    }
}
