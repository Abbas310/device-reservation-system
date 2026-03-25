using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Vanrise_Web.Models
{
    public class DeviceReportRowDto
    {
        public string DeviceName { get; set; }
        public string Status { get; set; }   // "Reserved" / "Unreserved"
        public int Count { get; set; }
    }
}