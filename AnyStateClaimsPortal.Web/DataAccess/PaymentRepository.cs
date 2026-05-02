using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using AnyStateClaimsPortal.Web.Models;

namespace AnyStateClaimsPortal.Web.DataAccess
{
    public class PaymentRepository
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public PaymentRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["AnyStateClaimsDB"].ConnectionString;
            _commandTimeout = int.TryParse(ConfigurationManager.AppSettings["CommandTimeout"], out int t) ? t : 30;
        }

        public List<PaymentListItem> GetPaymentsByClaimId(int claimId)
        {
            var payments = new List<PaymentListItem>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM vw_PaymentSummary WHERE ClaimId = @ClaimId", conn))
            {
                cmd.CommandTimeout = _commandTimeout;
                cmd.Parameters.AddWithValue("@ClaimId", claimId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        payments.Add(new PaymentListItem
                        {
                            PaymentId = Convert.ToInt32(reader["PaymentId"]),
                            ClaimId = Convert.ToInt32(reader["ClaimId"]),
                            Amount = Convert.ToDecimal(reader["Amount"]),
                            PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                            PaymentType = reader["PaymentType"].ToString(),
                            Status = reader["Status"].ToString()
                        });
                }
            }

            return payments;
        }

        public (int Count, decimal Total) ProcessPaymentBatch(string approvedBy)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("usp_ProcessPaymentBatch", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);
                var countParam = new SqlParameter("@ProcessedCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var totalParam = new SqlParameter("@TotalAmount", SqlDbType.Decimal) { Direction = ParameterDirection.Output, Precision = 18, Scale = 2 };
                cmd.Parameters.Add(countParam);
                cmd.Parameters.Add(totalParam);
                conn.Open();
                cmd.ExecuteNonQuery();
                return (Convert.ToInt32(countParam.Value), Convert.ToDecimal(totalParam.Value));
            }
        }
    }
}
