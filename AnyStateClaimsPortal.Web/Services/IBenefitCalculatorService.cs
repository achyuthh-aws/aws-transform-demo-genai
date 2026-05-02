using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace AnyStateClaimsPortal.Web.Services
{
    [ServiceContract(Namespace = "http://anystateportal.gov/benefits")]
    public interface IBenefitCalculatorService
    {
        [OperationContract]
        BenefitCalculationResult CalculateBenefit(BenefitCalculationRequest request);

        [OperationContract]
        decimal GetStateAverageWeeklyWage();

        [OperationContract]
        BenefitScheduleInfo GetBenefitSchedule();
    }

    [DataContract]
    public class BenefitCalculationRequest
    {
        [DataMember] public int EmployeeId { get; set; }
        [DataMember] public string InjuryType { get; set; }
    }

    [DataContract]
    public class BenefitCalculationResult
    {
        [DataMember] public decimal WeeklyBenefitAmount { get; set; }
        [DataMember] public decimal AnnualSalary { get; set; }
        [DataMember] public decimal BenefitRate { get; set; }
        [DataMember] public decimal DependentSupplement { get; set; }
        [DataMember] public decimal LongevityBonus { get; set; }
        [DataMember] public string InjuryType { get; set; }
        [DataMember] public int Dependents { get; set; }
        [DataMember] public int YearsOfService { get; set; }
        [DataMember] public bool IsSuccess { get; set; }
        [DataMember] public string ErrorMessage { get; set; }
    }

    [DataContract]
    public class BenefitScheduleInfo
    {
        [DataMember] public decimal StateAvgWeekly { get; set; }
        [DataMember] public decimal MaxBenefit { get; set; }
        [DataMember] public decimal MinBenefit { get; set; }
        [DataMember] public Dictionary<string, decimal> Rates { get; set; }
    }
}
