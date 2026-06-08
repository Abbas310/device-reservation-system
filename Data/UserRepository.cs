using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Vanrise_Web.Models;

namespace Vanrise_Web.Data
{
    public class UserRepository
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public UserDto GetByUsername(string username)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.User_GetByUsername", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        return new UserDto
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Username = Convert.ToString(r["Username"]),
                            PasswordHash = Convert.ToString(r["PasswordHash"]),
                            Role = Convert.ToString(r["Role"])
                        };
                    }
                }
            }
            return null;
        }

        public bool Add(string username, string passwordHash, string role)
        {
            try
            {
                using (var con = new SqlConnection(_cs))
                using (var cmd = new SqlCommand("dbo.User_Add", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@Role", role);

                    con.Open();
                    cmd.ExecuteScalar();
                    return true;
                }
            }
            catch (SqlException)
            {
                
                return false;
            }
        }
    }
}