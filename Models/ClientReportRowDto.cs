using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Vanrise_Web.Models
{
    public class ClientReportRowDto
    {
        public int Type { get; set; }          // 1 or 2
        public int ClientCount { get; set; }   // number of clients
    }
}