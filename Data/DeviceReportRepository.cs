using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Vanrise_Web.Models;

namespace Vanrise_Web.Data
{
    public class DeviceReportRepository
    {
        private readonly string _cs =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<DeviceReportRowDto> Get(int? deviceId, string status)
        {
            // normalize status coming from UI: "reserved" / "unreserved" / ""
            string s = string.IsNullOrWhiteSpace(status) ? null : status.Trim().ToLowerInvariant();

            var list = new List<DeviceReportRowDto>();

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.DeviceReport_CountPhoneNumbersByStatus", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DeviceId", (object)deviceId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object)s ?? DBNull.Value);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new DeviceReportRowDto
                        {
                            DeviceName = Convert.ToString(r["DeviceName"]),
                            Status = Convert.ToString(r["Status"]),
                            Count = Convert.ToInt32(r["Count"])
                        });
                    }
                }
            }

            return list;
        }
    }
}