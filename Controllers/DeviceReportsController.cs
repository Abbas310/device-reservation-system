using System.Collections.Generic;
using System.Web.Http;
using Vanrise_Web.Data;
using Vanrise_Web.Models;

namespace Vanrise_Web.Controllers
{
    public class DeviceReportsController : ApiController
    {
        private readonly DeviceReportRepository _repo = new DeviceReportRepository();

        // GET /api/devicereports?deviceId=1&status=reserved
        [HttpGet]
        public IEnumerable<DeviceReportRowDto> Get(int? deviceId = null, string status = null)
        {
            return _repo.Get(deviceId, status);
        }
    }
}