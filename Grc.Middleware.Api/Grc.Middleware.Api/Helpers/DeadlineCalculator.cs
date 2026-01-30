namespace Grc.Middleware.Api.Helpers {
    public static class DeadlineCalculator {

        public static DateTime CalculateDeadline(
            string frequencyName,
            DateTime periodStart,
            DateTime periodEnd,
            int requiredSubmissionDay) {

            switch (frequencyName.ToUpperInvariant()) {
                case "DAILY":
                    // For daily, deadline is the same day
                    return periodEnd;

                case "WEEKLY":
                    // RequiredSubmissionDay: 1=Monday, 2=Tuesday, ..., 6=Saturday
                    if (requiredSubmissionDay < 1 || requiredSubmissionDay > 6)
                        return periodEnd; // Default to period end if invalid

                    // Get the day of week (1=Monday, 6=Saturday)
                    DayOfWeek targetDay = requiredSubmissionDay switch {
                        1 => DayOfWeek.Monday,
                        2 => DayOfWeek.Tuesday,
                        3 => DayOfWeek.Wednesday,
                        4 => DayOfWeek.Thursday,
                        5 => DayOfWeek.Friday,
                        6 => DayOfWeek.Saturday,
                        _ => DayOfWeek.Sunday
                    };

                    // Find the target day in the week
                    int daysFromMonday = (int)targetDay - (int)DayOfWeek.Monday;
                    DateTime deadline = periodStart.AddDays(daysFromMonday);

                    // Ensure deadline is within the period
                    if (deadline < periodStart || deadline > periodEnd)
                        return periodEnd;

                    return deadline;

                case "MONTHLY":
                    // RequiredSubmissionDay: 1-31 (day of month)
                    if (requiredSubmissionDay < 1 || requiredSubmissionDay > 31)
                        return periodEnd;

                    // Handle months with fewer days
                    int maxDayInMonth = DateTime.DaysInMonth(periodStart.Year, periodStart.Month);
                    int actualDay = Math.Min(requiredSubmissionDay, maxDayInMonth);

                    return new DateTime(periodStart.Year, periodStart.Month, actualDay);

                case "QUARTERLY":
                    // RequiredSubmissionDay: 1-31 (day of the last month in quarter)
                    if (requiredSubmissionDay < 1 || requiredSubmissionDay > 31)
                        return periodEnd;

                    // Last month of the quarter
                    int lastMonthOfQuarter = periodEnd.Month;
                    int maxDayInLastMonth = DateTime.DaysInMonth(periodEnd.Year, lastMonthOfQuarter);
                    int actualDayQuarter = Math.Min(requiredSubmissionDay, maxDayInLastMonth);

                    return new DateTime(periodEnd.Year, lastMonthOfQuarter, actualDayQuarter);

                case "BIANNUAL":
                case "SEMIANNUAL":
                case "HALF-YEARLY":
                    // RequiredSubmissionDay: 1-31 (day of the last month in half-year)
                    if (requiredSubmissionDay < 1 || requiredSubmissionDay > 31)
                        return periodEnd;

                    int lastMonthOfHalf = periodEnd.Month;
                    int maxDayInHalf = DateTime.DaysInMonth(periodEnd.Year, lastMonthOfHalf);
                    int actualDayHalf = Math.Min(requiredSubmissionDay, maxDayInHalf);

                    return new DateTime(periodEnd.Year, lastMonthOfHalf, actualDayHalf);

                case "ANNUAL":
                case "YEARLY":
                    // RequiredSubmissionDay: 1-12 (month of the year)
                    if (requiredSubmissionDay < 1 || requiredSubmissionDay > 12)
                        return periodEnd;

                    // Use last day of the specified month
                    int daysInMonth = DateTime.DaysInMonth(periodEnd.Year, requiredSubmissionDay);
                    return new DateTime(periodEnd.Year, requiredSubmissionDay, daysInMonth);

                case "BIENNIAL":
                case "TRIENNIAL":
                    // RequiredSubmissionDay: 1-12 (month of the last year in period)
                    if (requiredSubmissionDay < 1 || requiredSubmissionDay > 12)
                        return periodEnd;

                    int daysInMonthMulti = DateTime.DaysInMonth(periodEnd.Year, requiredSubmissionDay);
                    return new DateTime(periodEnd.Year, requiredSubmissionDay, daysInMonthMulti);

                default:
                    return periodEnd;
            }
        }
    }
}
