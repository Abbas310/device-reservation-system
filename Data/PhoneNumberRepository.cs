using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Vanrise_Web.Models;
using Vanrise_Web.Models;

namespace Vanrise_Web.Data
{
    public class PhoneNumberRepository
    {
        private readonly string _cs =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<PhoneNumberDto> GetAll()
        {
            var list = new List<PhoneNumberDto>();

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumber_GetAll", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }

            return list;
        }

        public List<PhoneNumberDto> GetFiltered(string number, int? deviceId)
        {
            var list = new List<PhoneNumberDto>();

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumber_GetFiltered", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Number", (object)number ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DeviceId", (object)deviceId ?? DBNull.Value);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) list.Add(Map(r));
                }
            }

            return list;
        }

        public List<PhoneNumberDto> GetAvailable()
        {
            var list = new List<PhoneNumberDto>();

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumber_GetAvailable", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new PhoneNumberDto
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Number = Convert.ToString(r["Number"])
                        });
                    }
                }
            }

            return list;
        }

        public PhoneNumberDto Add(string number, int deviceId)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumber_Add", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Number", number);
                cmd.Parameters.AddWithValue("@DeviceId", deviceId);

                con.Open();
                var id = Convert.ToInt32(cmd.ExecuteScalar());

                return new PhoneNumberDto { Id = id, Number = number, DeviceId = deviceId };
            }
        }

        public bool Update(int id, string number, int deviceId)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumber_Update", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Number", number);
                cmd.Parameters.AddWithValue("@DeviceId", deviceId);

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool Delete(int id)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumber_Delete", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static PhoneNumberDto Map(IDataRecord r)
        {
            return new PhoneNumberDto
            {
                Id = Convert.ToInt32(r["Id"]),
                Number = Convert.ToString(r["Number"]),
                DeviceId = Convert.ToInt32(r["DeviceId"]),
                DeviceName = Convert.ToString(r["DeviceName"])
            };
        }
    }
}
