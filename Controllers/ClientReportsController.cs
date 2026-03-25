using System.Collections.Generic;
using System.Web.Http;
using Vanrise_Web.Data;
using Vanrise_Web.Models;

namespace Vanrise_Web.Controllers
{
    public class ClientReportsController : ApiController
    {
        private readonly ClientRepository _repo = new ClientRepository();

        // GET /api/clientreports?type=1
        [HttpGet]
        public IEnumerable<ClientReportRowDto> Get(int? type = null)
        {
            return _repo.GetCountByType(type);
        }
    }
}