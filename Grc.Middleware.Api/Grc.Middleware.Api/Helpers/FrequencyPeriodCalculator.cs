namespace Grc.Middleware.Api.Helpers {
    public static class FrequencyPeriodCalculator {
        public static (DateTime start, DateTime end) GetCurrentPeriod(string frequencyName, DateTime today) {
            today = today.Date;

            switch (frequencyName.ToUpperInvariant()) {
                case "DAILY":
                    return (today, today);

                case "WEEKLY": {
                        int daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                        DateTime monday = today.AddDays(-daysSinceMonday);
                        DateTime sunday = monday.AddDays(6);
                        return (monday, sunday);
                    }

                case "MONTHLY":
                    return (new DateTime(today.Year, today.Month, 1),
                            new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)));

                case "QUARTERLY": {
                        int quarter = (today.Month - 1) / 3;
                        int startMonth = quarter * 3 + 1;
                        int endMonth = startMonth + 2;
                        return (new DateTime(today.Year, startMonth, 1),
                                new DateTime(today.Year, endMonth, DateTime.DaysInMonth(today.Year, endMonth)));
                    }

                case "BIANNUAL":
                case "SEMIANNUAL":
                case "HALF-YEARLY":
                    if (today.Month <= 6)
                        return (new DateTime(today.Year, 1, 1), new DateTime(today.Year, 6, 30));
                    else
                        return (new DateTime(today.Year, 7, 1), new DateTime(today.Year, 12, 31));

                case "ANNUAL":
                case "YEARLY":
                    return (new DateTime(today.Year, 1, 1), new DateTime(today.Year, 12, 31));
                case "BIENNIAL": {
                        int yearOffset = today.Year % 2;
                        int startYear = today.Year - yearOffset;
                        int endYear = startYear + 1;
                        return (new DateTime(startYear, 1, 1), new DateTime(endYear, 12, 31));
                }
                case "TRIENNIAL": {
                        int yearOffset = today.Year % 3;
                        int startYear = today.Year - yearOffset;
                        int endYear = startYear + 2;
                        return (new DateTime(startYear, 1, 1), new DateTime(endYear, 12, 31));
                }
                default:
                    throw new NotSupportedException($"Unsupported frequency: {frequencyName}");
            }
        }
    }

}
