using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Vanrise_Web.Models;
using Vanrise_Web.Models;

namespace Vanrise_Web.Data
{
    public class ClientRepository
    {
        private readonly string _cs =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<ClientDto> GetAll()
        {
            var list = new List<ClientDto>();

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Client_GetAll", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(Map(r));
                }
            }

            return list;
        }

        public List<ClientDto> GetFiltered(string search, int? type)
        {
            var list = new List<ClientDto>();

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Client_GetFiltered", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Search", (object)search ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Type", (object)type ?? DBNull.Value);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        list.Add(Map(r));
                }
            }

            return list;
        }

        public List<ClientReportRowDto> GetCountByType(int? type)
        {
            var list = new List<ClientReportRowDto>();

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.ClientReport_CountByType", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Type", (object)type ?? DBNull.Value);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new ClientReportRowDto
                        {
                            Type = Convert.ToInt32(r["Type"]),
                            ClientCount = Convert.ToInt32(r["ClientCount"])
                        });
                    }
                }
            }

            return list;
        }

        public ClientDto Add(ClientDto c)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Client_Add", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Type", c.Type);
                cmd.Parameters.AddWithValue("@BirthDate", (object)c.BirthDate ?? DBNull.Value);

                con.Open();
                var id = Convert.ToInt32(cmd.ExecuteScalar());
                c.Id = id;
                return c;
            }
        }

        public bool Update(ClientDto c)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Client_Update", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", c.Id);
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Type", c.Type);
                cmd.Parameters.AddWithValue("@BirthDate", (object)c.BirthDate ?? DBNull.Value);

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public bool Delete(int id)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Client_Delete", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private ClientDto Map(IDataRecord r)
        {
            return new ClientDto
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = Convert.ToString(r["Name"]),
                Type = Convert.ToInt32(r["Type"]),
                BirthDate = r["BirthDate"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(r["BirthDate"])
            };
        }
    }
}
