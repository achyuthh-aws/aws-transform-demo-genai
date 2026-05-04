using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using AnyStateClaimsPortal.Web.Models;
using Microsoft.Data.SqlClient;


namespace AnyStateClaimsPortal.Web.DataAccess
{
    public class ClaimsRepository
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public ClaimsRepository()
        {
            _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AnyStateClaimsDB"]?.ConnectionString ?? string.Empty;
            int t;
            _commandTimeout = int.TryParse(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"], out t) ? t : 30;
        }

        public SearchClaimsResult SearchClaims(
            string searchTerm, string status, int? agencyId, string injuryType,
            string priority, DateTime? dateFrom, DateTime? dateTo,
            int? adjusterId, bool? isLitigated, int pageNumber, int pageSize)
        {
            var claims = new List<ClaimListItemViewModel>();
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 25;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("usp_SearchClaims", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Parameters.AddWithValue("@SearchTerm", (object)searchTerm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AgencyId", (object)agencyId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InjuryType", (object)injuryType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Priority", (object)priority ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateFrom", (object)dateFrom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateTo", (object)dateTo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AdjusterId", (object)adjusterId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsLitigated", (object)isLitigated ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@SortColumn", "CreatedDate");
                cmd.Parameters.AddWithValue("@SortDirection", "DESC");
                var totalParam = new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(totalParam);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        claims.Add(MapClaimListItem(reader));
                }

                var result = new SearchClaimsResult();
                result.Claims = claims;
                result.TotalCount = totalParam.Value != DBNull.Value ? Convert.ToInt32(totalParam.Value) : 0;
                return result;
            }
        }

        public DashboardViewModel GetDashboardData()
        {
            var dashboard = new DashboardViewModel
            {
                StatusSummaries = new List<StatusSummary>(),
                RecentClaims = new List<RecentClaimViewModel>(),
                AgencySummaries = new List<AgencySummary>()
            };

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("usp_GetClaimsDashboard", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    // Result Set 1: Status summary (Status, ClaimCount, TotalWeeklyBenefits, TotalPaid, TotalReserves)
                    while (reader.Read())
                    {
                        var s = new StatusSummary
                        {
                            Status = reader["Status"].ToString(),
                            ClaimCount = Convert.ToInt32(reader["ClaimCount"]),
                            TotalWeeklyBenefits = Convert.ToDecimal(reader["TotalWeeklyBenefits"]),
                            TotalPaid = Convert.ToDecimal(reader["TotalPaid"])
                        };
                        dashboard.StatusSummaries.Add(s);
                        dashboard.TotalClaims += s.ClaimCount;
                        dashboard.TotalPaid += s.TotalPaid;
                    }

                    // Result Set 2: Recent claims
                    if (reader.NextResult())
                        while (reader.Read())
                            dashboard.RecentClaims.Add(new RecentClaimViewModel
                            {
                                ClaimId = Convert.ToInt32(reader["ClaimId"]),
                                ClaimNumber = reader["ClaimNumber"].ToString(),
                                InjuryDate = Convert.ToDateTime(reader["InjuryDate"]),
                                InjuryType = reader["InjuryType"].ToString(),
                                Status = reader["Status"].ToString(),
                                Priority = reader["Priority"].ToString(),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                AgencyName = reader["AgencyName"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });

                    // Result Set 3: Agency summary
                    if (reader.NextResult())
                        while (reader.Read())
                            dashboard.AgencySummaries.Add(new AgencySummary
                            {
                                AgencyName = reader["AgencyName"].ToString(),
                                AgencyCode = reader["AgencyCode"].ToString(),
                                RiskCategory = reader["RiskCategory"].ToString(),
                                ClaimCount = Convert.ToInt32(reader["ClaimCount"]),
                                TotalPaid = Convert.ToDecimal(reader["TotalPaid"]),
                                TotalMedical = Convert.ToDecimal(reader["TotalMedical"])
                            });
                }
            }

            return dashboard;
        }

        public decimal CalculateWeeklyBenefit(int employeeId, string injuryType)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("usp_CalculateWeeklyBenefit", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@InjuryType", injuryType);
                var outputParam = new SqlParameter("@WeeklyBenefit", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Output,
                    Precision = 18,
                    Scale = 2
                };
                cmd.Parameters.Add(outputParam);
                conn.Open();
                cmd.ExecuteNonQuery();
                return (decimal)outputParam.Value;
            }
        }

        private ClaimListItemViewModel MapClaimListItem(SqlDataReader reader)
        {
            return new ClaimListItemViewModel
            {
                ClaimId = Convert.ToInt32(reader["ClaimId"]),
                ClaimNumber = reader["ClaimNumber"].ToString(),
                EmployeeName = reader["EmployeeName"].ToString(),
                EmployeeNumber = reader["EmployeeNumber"].ToString(),
                Status = reader["Status"].ToString(),
                InjuryDate = Convert.ToDateTime(reader["InjuryDate"]),
                InjuryType = reader["InjuryType"].ToString(),
                BodyPartAffected = reader["BodyPartAffected"].ToString(),
                AgencyName = reader["AgencyName"].ToString(),
                AgencyCode = reader["AgencyCode"].ToString(),
                Priority = reader["Priority"].ToString(),
                AdjusterName = reader["AdjusterName"] as string,
                WeeklyBenefitAmount = reader["WeeklyBenefitAmount"] as decimal?,
                TotalPaidAmount = reader["TotalPaidAmount"] as decimal? ?? 0m,
                TotalMedicalCost = reader["TotalMedicalCost"] as decimal? ?? 0m,
                IsLitigated = Convert.ToBoolean(reader["IsLitigated"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };
        }
    }

    public class SearchClaimsResult
    {
        public List<ClaimListItemViewModel> Claims { get; set; }
        public int TotalCount { get; set; }
    }
}
