using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using AnyStateClaimsPortal.Web.Models;

namespace AnyStateClaimsPortal.Web.DataAccess
{
    public class ReportRepository
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public ReportRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["AnyStateClaimsDB"].ConnectionString;
            int t;
            _commandTimeout = int.TryParse(ConfigurationManager.AppSettings["CommandTimeout"], out t) ? t : 30;
        }

        public List<AgencyReportItem> GetAgencyClaimsReport()
        {
            var items = new List<AgencyReportItem>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM vw_AgencyClaimsReport", conn))
            {
                cmd.CommandTimeout = _commandTimeout;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        items.Add(new AgencyReportItem
                        {
                            AgencyName = reader["AgencyName"].ToString(),
                            TotalClaims = Convert.ToInt32(reader["TotalClaims"]),
                            SubmittedCount = Convert.ToInt32(reader["OpenClaims"]),
                            ClosedCount = Convert.ToInt32(reader["ClosedClaims"]),
                            TotalPaidAmount = Convert.ToDecimal(reader["TotalPaid"]),
                            LossRatio = Convert.ToDecimal(reader["AvgProcessingDays"])
                        });
            }

            return items;
        }

        public FinancialSummaryViewModel GetFinancialSummary(int fiscalYear, int? agencyId)
        {
            var summary = new FinancialSummaryViewModel();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("usp_GetClaimFinancialSummary", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Parameters.AddWithValue("@FiscalYear", fiscalYear);
                cmd.Parameters.AddWithValue("@AgencyId", (object)agencyId ?? DBNull.Value);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        summary.TotalPaid = Convert.ToDecimal(reader["TotalPaid"]);
                        summary.TotalReserved = Convert.ToDecimal(reader["TotalReserved"]);
                        summary.ClaimCount = Convert.ToInt32(reader["ClaimCount"]);
                    }

                    if (reader.NextResult())
                    {
                        summary.MonthlyBreakdown = new List<MonthlyFinancial>();
                        while (reader.Read())
                            summary.MonthlyBreakdown.Add(new MonthlyFinancial
                            {
                                Month = reader["Month"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"])
                            });
                    }

                    if (reader.NextResult())
                    {
                        summary.ByInjuryType = new List<InjuryTypeFinancial>();
                        while (reader.Read())
                            summary.ByInjuryType.Add(new InjuryTypeFinancial
                            {
                                InjuryType = reader["InjuryType"].ToString(),
                                TotalPaid = Convert.ToDecimal(reader["TotalPaid"]),
                                ClaimCount = Convert.ToInt32(reader["ClaimCount"])
                            });
                    }
                }
            }

            return summary;
        }

        public List<AgingBucketItem> GetOpenClaimsAging()
        {
            var items = new List<AgingBucketItem>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(
                "SELECT AgingBucket, COUNT(*) AS ClaimCount, SUM(TotalReserved) AS TotalReserved " +
                "FROM vw_OpenClaimsAging GROUP BY AgingBucket ORDER BY AgingBucket", conn))
            {
                cmd.CommandTimeout = _commandTimeout;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        items.Add(new AgingBucketItem
                        {
                            AgingBucket = reader["AgingBucket"].ToString(),
                            ClaimCount = Convert.ToInt32(reader["ClaimCount"]),
                            TotalReserved = Convert.ToDecimal(reader["TotalReserved"])
                        });
            }

            return items;
        }
    }
}
