using System;
using System.Collections.Generic;
using System.Data;
using Vanrise_Web.Models;
using Vanrise_Web.Models;

namespace Vanrise_Web.Data
{
    public static class DeviceMapper
    {
        // did delegate using Func
        public static List<T> MapList<T>(
            IDataReader reader,
            Func<IDataRecord, T> mapper)
        {
            var list = new List<T>();

            while (reader.Read())
            {
                list.Add(mapper(reader));
            }

            return list;
        }

        // actual device mapper
        public static DeviceDto MapDevice(IDataRecord r)
        {
            return new DeviceDto
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = Convert.ToString(r["Name"])
            };
        }
    }
}
