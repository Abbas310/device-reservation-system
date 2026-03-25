using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Vanrise_Web.Models;

namespace Vanrise_Web.Data
{
    public class DeviceRepository
    {
        private readonly string _cs =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<DeviceDto> GetAll()
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Device_GetAll", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (var r = cmd.ExecuteReader())
                {
                    // delegate / Func mapping
                    return DeviceMapper.MapList(r, DeviceMapper.MapDevice);
                }
            }
        }

        public List<DeviceDto> GetFiltered(string search)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Device_GetFiltered", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Search", (object)search ?? DBNull.Value);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    // delegate / Func mapping
                    return DeviceMapper.MapList(r, DeviceMapper.MapDevice);
                }
            }
        }

        public DeviceDto Add(string name)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Device_Add", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", name);

                con.Open();
                var newId = Convert.ToInt32(cmd.ExecuteScalar());

                return new DeviceDto { Id = newId, Name = name };
            }
        }

        public bool Update(int id, string name)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Device_Update", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", name);

                con.Open();
                var affected = Convert.ToInt32(cmd.ExecuteScalar());
                return affected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("dbo.Device_Delete", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                var affected = Convert.ToInt32(cmd.ExecuteScalar());
                return affected > 0;
            }
        }
    }
}
