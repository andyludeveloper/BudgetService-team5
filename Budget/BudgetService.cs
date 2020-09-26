using System;
using System.Linq;
using NSubstitute;

namespace Budget
{
    public class BudgetService
    {
        private readonly IBudgetRepo _repo;

        public BudgetService(IBudgetRepo repo)
        {
            _repo = repo;
        }

        public double Query(DateTime start, DateTime end)
        {
            if (start > end)
            {
                return 0;
            }

            var budgets = _repo.GetAll();
            if (!budgets.Any())
            {
                return 0;
            }

            var totalBudget = 0;
            foreach (var budget in budgets)
            {
                var daysInMonth = Days(budget);
                if (start.ToString("yyyyMM") == end.ToString("yyyyMM"))
                {
                    totalBudget += budget.Amount / daysInMonth * ((end - start).Days + 1);
                }
                else
                {
                    if (budget.YearMonth == start.ToString("yyyyMM"))
                    {
                        var lastOfMonth = new DateTime(budget.FirstDay().Year, budget.FirstDay().Month, daysInMonth);
                        totalBudget += budget.Amount / daysInMonth * ((lastOfMonth - start).Days + 1);
                    }
                    else if (budget.YearMonth == end.ToString("yyyyMM"))
                    {
                        totalBudget += budget.Amount / daysInMonth * ((end - budget.FirstDay()).Days + 1);
                    }
                    else if (budget.FirstDay() >= start && budget.FirstDay() <= end)
                    {
                        totalBudget += budget.Amount;
                    }
                }
            }

            return totalBudget;
        }

        private static int Days(Budget budget)
        {
            var daysInMonth = DateTime.DaysInMonth(budget.FirstDay().Year, budget.FirstDay().Month);
            return daysInMonth;
        }
    }
}