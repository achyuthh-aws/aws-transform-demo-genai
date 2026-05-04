using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using AnyStateClaimsPortal.Web.Models;

namespace AnyStateClaimsPortal.Web.DataAccess
{
    public class MedicalRepository
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public MedicalRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["AnyStateClaimsDB"].ConnectionString;
            int t;
            _commandTimeout = int.TryParse(ConfigurationManager.AppSettings["CommandTimeout"], out t) ? t : 30;
        }

        public List<TreatmentViewModel> GetTreatmentHistory(int claimId)
        {
            var treatments = new List<TreatmentViewModel>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("usp_GetMedicalTreatmentHistory", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = _commandTimeout;
                cmd.Parameters.AddWithValue("@ClaimId", claimId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        treatments.Add(new TreatmentViewModel
                        {
                            TreatmentId = Convert.ToInt32(reader["TreatmentId"]),
                            TreatmentDate = Convert.ToDateTime(reader["TreatmentDate"]),
                            ProviderName = reader["Provider"].ToString(),
                            Description = reader["Description"].ToString(),
                            BilledAmount = Convert.ToDecimal(reader["Cost"]),
                            Status = reader["Status"].ToString()
                        });
                }
            }

            return treatments;
        }
    }
}
