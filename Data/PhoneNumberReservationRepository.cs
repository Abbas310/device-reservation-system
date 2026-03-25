using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Vanrise_Web.Models;

namespace Vanrise_Web.Data
{
    public class PhoneNumberReservationRepository
    {

        public int Reserve(int clientId, int phoneNumberId)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumberReservation_Reserve", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClientId", clientId);
                cmd.Parameters.AddWithValue("@PhoneNumberId", phoneNumberId);

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Unreserve(int reservationId)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumberReservation_Unreserve", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReservationId", reservationId);

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
        private readonly string _cs =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<PhoneNumberReservationDto> GetAll()
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumberReservation_GetAll", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (var r = cmd.ExecuteReader())
                {
                    var list = new List<PhoneNumberReservationDto>();
                    while (r.Read()) list.Add(Map(r));
                    return list;
                }
            }
        }

        public List<PhoneNumberReservationDto> GetFiltered(int? clientId, int? phoneNumberId)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumberReservation_GetFiltered", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClientId", (object)clientId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumberId", (object)phoneNumberId ?? DBNull.Value);

                con.Open();

                using (var r = cmd.ExecuteReader())
                {
                    var list = new List<PhoneNumberReservationDto>();
                    while (r.Read()) list.Add(Map(r));
                    return list;
                }
            }
        }

        public List<ActiveReservationDto> GetActiveByClient(int clientId)
        {
            var list = new List<ActiveReservationDto>();

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.PhoneNumberReservation_GetActiveByClient", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new ActiveReservationDto
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            PhoneNumberId = Convert.ToInt32(r["PhoneNumberId"]),
                            PhoneNumber = Convert.ToString(r["PhoneNumber"])
                        });
                    }
                }
            }

            return list;
        }

        private static PhoneNumberReservationDto Map(IDataRecord r)
        {
            return new PhoneNumberReservationDto
            {
                Id = Convert.ToInt32(r["Id"]),
                ClientId = Convert.ToInt32(r["ClientId"]),
                ClientName = Convert.ToString(r["ClientName"]),
                PhoneNumberId = Convert.ToInt32(r["PhoneNumberId"]),
                PhoneNumber = Convert.ToString(r["PhoneNumber"]),
                BED = Convert.ToDateTime(r["BED"]),
                EED = r["EED"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["EED"])
            };
        }
    }
}
