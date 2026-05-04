using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;

using AnyStateClaimsPortal.Web.BusinessLogic;
using AnyStateClaimsPortal.Web.DataAccess;

namespace AnyStateClaimsPortal.Web.Services
{
    public class BenefitCalculatorService : IBenefitCalculatorService
    {
        private readonly ClaimsRepository _claimsRepo = new ClaimsRepository();
        private readonly BenefitCalculationEngine _calcEngine = new BenefitCalculationEngine();

        public BenefitCalculationResult CalculateBenefit(BenefitCalculationRequest request)
        {
            try
            {
                decimal weeklyBenefit = _claimsRepo.CalculateWeeklyBenefit(request.EmployeeId, request.InjuryType);
                return new BenefitCalculationResult
                {
                    WeeklyBenefitAmount = weeklyBenefit,
                    InjuryType = request.InjuryType,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new BenefitCalculationResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public decimal GetStateAverageWeeklyWage()
        {
            decimal wage;
            return decimal.TryParse(System.Configuration.ConfigurationManager.AppSettings["StateAvgWeeklyWage"], out wage)
                ? wage
                : 1025.00m;
        }

        public BenefitScheduleInfo GetBenefitSchedule()
        {
            decimal stateAvg = GetStateAverageWeeklyWage();
            return new BenefitScheduleInfo
            {
                StateAvgWeekly = stateAvg,
                MaxBenefit = stateAvg * 1.5m,
                MinBenefit = stateAvg * 0.15m,
                Rates = new Dictionary<string, decimal>
                {
                    { "Medical", 0.60m },
                    { "Temporary", 0.6667m },
                    { "Permanent", 0.70m },
                    { "Fatal", 0.75m }
                }
            };
        }
    }
}
