using System;
using AnyStateClaimsPortal.Web.Models;

namespace AnyStateClaimsPortal.Web.BusinessLogic
{
    public class BenefitCalculationEngine
    {
        private const decimal StateAvgWeeklyWage = 1025.00m;

        public BenefitResult CalculateBenefit(decimal annualSalary, string injuryType, int dependents, int yearsOfService)
        {
            decimal baseRate;
            switch (injuryType)
            {
                case "Medical": baseRate = 0.60m; break;
                case "Temporary": baseRate = 0.6667m; break;
                case "Permanent": baseRate = 0.70m; break;
                case "Fatal": baseRate = 0.75m; break;
                default: baseRate = 0.60m; break;
            }

            decimal dependentSupplement = Math.Min(dependents * 0.02m, 0.10m);
            decimal longevityBonus = Math.Min((yearsOfService / 5) * 0.005m, 0.02m);
            decimal totalRate = baseRate + dependentSupplement + longevityBonus;

            decimal weeklyWage = annualSalary / 52m;
            decimal weeklyBenefit = weeklyWage * totalRate;

            decimal maxBenefit = StateAvgWeeklyWage * 1.5m;
            decimal minBenefit = StateAvgWeeklyWage * 0.15m;
            weeklyBenefit = Math.Max(Math.Min(weeklyBenefit, maxBenefit), minBenefit);

            return new BenefitResult
            {
                WeeklyBenefit = Math.Round(weeklyBenefit, 2),
                BenefitRate = totalRate,
                DependentSupplement = dependentSupplement,
                LongevityBonus = longevityBonus,
                MaxBenefit = maxBenefit,
                MinBenefit = minBenefit,
                StateAvgWeekly = StateAvgWeeklyWage
            };
        }
    }
}
