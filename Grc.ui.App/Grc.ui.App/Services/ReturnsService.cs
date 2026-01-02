using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public class ReturnsService : IReturnsService {

        public async Task<GeneralComplianceReturnStatistics> GetAllStatisticAsync(long userId, string ipAddress) {
            return await Task.FromResult(new GeneralComplianceReturnStatistics() {
                //..circulars
                TotalCircular = new() {
                    Authority = string.Empty,
                    Total = 12,
                    Open = 8,
                    Closed = 4,
                },
                CircularList = new List<CircularStatistics>() {
                    new() {
                        Authority = "BOU",
                        Open = 2,
                        Closed = 3,
                        Total = 5
                    },
                    new() {
                        Authority = "FIA",
                        Open = 1,
                        Closed = 3,
                        Total = 4
                    },
                    new() {
                        Authority = "CTA",
                        Open = 0,
                        Closed = 3,
                        Total = 3
                    }
                },
                //..returns
                TotalReturns = new() {
                    Period = string.Empty,
                    Total = 30,
                    Open = 25,
                    Closed = 5,
                },
                ReturnsList = new List<ReturnStatistics>() {
                    new() {
                        Period = "Today",
                        Total = 10,
                        Open = 3,
                        Closed = 7,
                    },
                    new() {
                        Period = "This Week",
                        Total = 12,
                        Open = 4,
                        Closed = 8,
                    },
                    new() {
                        Period = "This Month",
                        Total = 8,
                        Open = 3,
                        Closed = 5,
                    }
                },
                TotalTasks = new() {
                    Period = string.Empty,
                    Total = 10,
                    Open = 2,
                    Closed = 8,
                },
                TaskLists = new List<TaskStatistics>() {
                    new() {
                        Period = "Today",
                        Total = 10,
                        Open = 3,
                        Closed = 7,
                    },
                    new() {
                        Period = "This Week",
                        Total = 12,
                        Open = 4,
                        Closed = 8,
                    },
                    new() {
                        Period = "This Month",
                        Total = 8,
                        Open = 3,
                        Closed = 5,
                    }
                }
            });
        }

        #region Circular Statistics

        public async Task<ComplianceCircularStatistics> GetCircularStatisticAsync(long userId, string ipAddress) {
            return await Task.FromResult(new ComplianceCircularStatistics() {
                Totals = new() {
                    Authority = string.Empty,
                    Total = 12,
                    Open = 8,
                    Closed = 4,
                },
                CircularList = new List<CircularStatistics>() {
                    new() {
                        Authority = "BOU",
                        Open = 2,
                        Closed = 3,
                        Total = 5
                    },
                    new() {
                        Authority = "FIA",
                        Open = 1,
                        Closed = 3,
                        Total = 4
                    },
                    new() {
                        Authority = "CTA",
                        Open = 0,
                        Closed = 3,
                        Total = 3
                    }
                },
            });
        }

        public async Task<ComplianceMinCircularStatistics> GetClosedCircularStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinCircularStatistics() {
                CircularList = new List<CircularStatistics>() {
                    new()
                    {
                        Period = GetMonthPeriod(),
                        Closed = 8,
                        Total = 10
                    },
                    new()
                    {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Closed = 54,
                        Total = 120
                    },
                    new()
                    {
                        Period = GetThisWeekPeriod(),
                        Closed = 28,
                        Total = 30
                    },
                    new()
                    {
                        Period = GetLastWeekPeriod(),
                        Closed = 26,
                        Total = 30
                    }
                },
            });
        }

        public async Task<ComplianceMinCircularStatistics> GetOpenCircularStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinCircularStatistics() {
                CircularList = new List<CircularStatistics>() {
                    new()
                    {
                        Period = GetMonthPeriod(),
                        Open = 18,
                    },
                    new()
                    {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Open = 5,
                    },
                    new()
                    {
                        Period = GetThisWeekPeriod(),
                        Open = 28,
                    },
                    new()
                    {
                        Period = GetLastWeekPeriod(),
                        Open = 30,
                    }
                },
            });
        }

        public async Task<ComplianceMinCircularStatistics> GetReceivedCircularStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinCircularStatistics() {
                CircularList = new List<CircularStatistics>() {
                    new()
                    {
                        Period = GetMonthPeriod(),
                        Total = 58
                    },
                    new()
                    {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Total = 10
                    },
                    new()
                    {
                        Period = GetThisWeekPeriod(),
                        Total = 30
                    },
                    new()
                    {
                        Period = GetLastWeekPeriod(),
                        Total = 30
                    }
                },
            });
        }

        public async Task<ComplianceMinCircularStatistics> GetBreachCircularStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinCircularStatistics() {
                CircularList = new List<CircularStatistics>() {
                    new()
                    {
                        Period = GetMonthPeriod(),
                        Breach = 58
                    },
                    new()
                    {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Breach = 10
                    },
                    new()
                    {
                        Period = GetThisWeekPeriod(),
                        Breach = 30
                    },
                    new()
                    {
                        Period = GetLastWeekPeriod(),
                        Breach = 30
                    }
                },
            });
        }

        #endregion

        #region Return Statistics

        public async Task<ComplianceReturnStatistics> GetReturnStatisticAsync(long userId, string ipAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceReturnStatistics() {
                Totals = new() {
                    Period = GetMonthPeriod(),
                    Total = 31,
                    Open = 7,
                    Closed = 25,
                    Breach = 3
                },
                ReturnsList = new List<ReturnStatistics>() {
                    new() {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Total = 10,
                        Open = 3,
                        Closed = 6,
                        Breach = 0
                    },
                    new() {
                        Period = GetThisWeekPeriod(),
                        Total = 12,
                        Open = 4,
                        Closed = 7,
                        Breach = 0
                    },
                    new() {
                        Period = GetLastWeekPeriod(),
                        Total = 12,
                        Open = 0,
                        Closed = 12,
                        Breach = 3
                    }
                }
            });
        }

        public async Task<ComplianceMinReturnStatistics> GetReturnReceivedStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinReturnStatistics() {
                ReturnsList = new List<ReturnStatistics>() {
                    new() {
                        Period = GetMonthPeriod(),
                        Total = 35
                    },
                    new() {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Total = 5
                    },
                    new() {
                        Period = GetThisWeekPeriod(),
                        Total = 35
                    },
                    new() {
                        Period = GetLastWeekPeriod(),
                        Total = 35
                    }
                }
            });
        }

        public async Task<ComplianceMinReturnStatistics> GetReturnTotalStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinReturnStatistics() {
                ReturnsList = new List<ReturnStatistics>() {
                    new() {
                        Period = GetMonthPeriod(),
                        Total = 35
                    },
                    new() {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Total = 5
                    },
                    new() {
                        Period = GetThisWeekPeriod(),
                        Total = 35
                    },
                    new() {
                        Period = GetLastWeekPeriod(),
                        Total = 35
                    }
                }
            });
        }

        public async Task<ComplianceMinReturnStatistics> GetReturnOpenStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinReturnStatistics() {
                ReturnsList = new List<ReturnStatistics>() {
                    new() {
                        Period = GetMonthPeriod(),
                        Open = 50
                    },
                    new() {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Open = 7
                    },
                    new() {
                        Period = GetThisWeekPeriod(),
                        Total = 20
                    },
                    new() {
                        Period = GetLastWeekPeriod(),
                        Total = 20
                    }
                }
            });
        }

        public async Task<ComplianceMinReturnStatistics> GetReturnSubmittedStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinReturnStatistics() {
                ReturnsList = new List<ReturnStatistics>() {
                    new() {
                        Period = GetMonthPeriod(),
                        Closed = 50
                    },
                    new() {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Closed = 7
                    },
                    new() {
                        Period = GetThisWeekPeriod(),
                        Closed = 9
                    },
                    new() {
                        Period = GetLastWeekPeriod(),
                        Closed = 15
                    }
                }
            });
        }

        public async Task<ComplianceMinReturnStatistics> GetReturnBreachStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinReturnStatistics() {
                ReturnsList = new List<ReturnStatistics>() {
                    new() {
                        Period = GetMonthPeriod(),
                        Breach = 3
                    },
                    new() {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Breach = 0
                    },
                    new() {
                        Period = GetThisWeekPeriod(),
                        Breach = 2
                    },
                    new() {
                        Period = GetLastWeekPeriod(),
                        Breach = 1
                    }
                }
            });
        }

        #endregion

        #region Tasks

        public async Task<ComplianceMinStatistics> GetClosedTaskStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinStatistics() {
                TaskLists = new List<TaskStatistics>() {
                    new()
                    {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Open = 2,
                        Closed = 8,
                        Failed = 0,
                        Total = 10
                    },
                    new()
                    {
                        Period = GetThisWeekPeriod(),
                        Open = 0,
                        Closed = 28,
                        Failed = 2,
                        Total = 30
                    },
                    new()
                    {
                        Period = GetLastWeekPeriod(),
                        Open = 0,
                        Closed = 26,
                        Failed = 4,
                        Total = 30
                    },
                    new()
                    {
                        Period = GetMonthPeriod(),
                        Open = 59,
                        Closed = 54,
                        Failed = 7,
                        Total = 120
                    }
                }
            });
        }

        public async Task<ComplianceMinStatistics> GetFailedTaskStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinStatistics() {
                TaskLists = new List<TaskStatistics>() {
                    new()
                    {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Open = 2,
                        Closed = 8,
                        Failed = 0,
                        Total = 10
                    },
                    new()
                    {
                        Period = GetThisWeekPeriod(),
                        Open = 0,
                        Closed = 28,
                        Failed = 2,
                        Total = 30
                    },
                    new()
                    {
                        Period = GetLastWeekPeriod(),
                        Open = 0,
                        Closed = 26,
                        Failed = 4,
                        Total = 30
                    },
                    new()
                    {
                        Period = GetMonthPeriod(),
                        Open = 59,
                        Closed = 54,
                        Failed = 7,
                        Total = 120
                    }
                }
            });
        }

        public async Task<ComplianceMinStatistics> GetOpenTaskStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinStatistics() {
                TaskLists = new List<TaskStatistics>() {
                    new()
                    {
                        Period = GetMonthPeriod(),
                        Open = 2,
                        Closed = 8,
                        Failed = 0,
                        Total = 10
                    },
                    new()
                    {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Open = 59,
                        Closed = 54,
                        Failed = 7,
                        Total = 120
                    },
                    new()
                    {
                        Period = GetThisWeekPeriod(),
                        Open = 0,
                        Closed = 28,
                        Failed = 2,
                        Total = 30
                    },
                    new()
                    {
                        Period = GetLastWeekPeriod(),
                        Open = 0,
                        Closed = 26,
                        Failed = 4,
                        Total = 30
                    }
                }
            });
        }

        public async Task<ComplianceMinStatistics> GetTotalTaskStatisticAsync(long userId, string iPAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceMinStatistics() {
                TaskLists = new List<TaskStatistics>() {
                    new()
                    {
                        Period =GetMonthPeriod(),
                        Open = 2,
                        Closed = 8,
                        Failed = 0,
                        Total = 10
                    },
                    new()
                    {
                        Period = $"TODAY {today:yyyy-MM-dd}", 
                        Open = 0,
                        Closed = 28,
                        Failed = 2,
                        Total = 30
                    },
                    new()
                    {
                        Period = GetThisWeekPeriod(),
                        Open = 0,
                        Closed = 26,
                        Failed = 4,
                        Total = 30
                    },
                    new()
                    {
                        Period = GetLastWeekPeriod(),
                        Open = 59,
                        Closed = 54,
                        Failed = 7,
                        Total = 120
                    }
                }
            });
        }

        public async Task<ComplianceTaskStatistics> GetTaskStatisticAsync(long userId, string ipAddress) {
            var today = DateTime.Today;
            return await Task.FromResult(new ComplianceTaskStatistics() {
                Totals = new() {
                    Period = GetMonthPeriod(),
                    Total = 31,
                    Open = 7,
                    Closed = 25,
                    Failed = 3,
                },
                TaskLists = new List<TaskStatistics>() {
                    new() {
                        Period = $"TODAY {today:yyyy-MM-dd}",
                        Total = 10,
                        Open = 3,
                        Closed = 6,
                        Failed = 0
                    },
                    new() {
                        Period = GetThisWeekPeriod(),
                        Total = 12,
                        Open = 4,
                        Closed = 7,
                        Failed = 0
                    },
                    new() {
                        Period = GetLastWeekPeriod(),
                        Total = 12,
                        Open = 0,
                        Closed = 12,
                        Failed = 3
                    }
                }
            });
        }

        #endregion

        #region Periods

        public static string GetThisWeekPeriod() {
            var today = DateTime.Today;

            // Calculate Monday of the current week
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var weekStart = today.AddDays(-diff);
            var weekEnd = weekStart.AddDays(6);

            return $"WEEK {weekStart:yyyy-MM-dd}/{weekEnd:yyyy-MM-dd}";
        }

        public static string GetLastWeekPeriod() {
            var today = DateTime.Today;

            // Calculate Monday of the current week
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var thisWeekStart = today.AddDays(-diff);

            // Last week is the 7 days before this week
            var lastWeekStart = thisWeekStart.AddDays(-7);
            var lastWeekEnd = thisWeekStart.AddDays(-1);

            return $"LAST WEEK {lastWeekStart:yyyy-MM-dd}/{lastWeekEnd:yyyy-MM-dd}";
        }

        public static string GetMonthPeriod() {
            var today = DateTime.Today;
            return today.ToString("MMM yyyy").ToUpper();
        }

        #endregion

    }
}
