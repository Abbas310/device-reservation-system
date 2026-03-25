using System.Configuration;
using System.Data.SqlClient;
using System.Web.Http;

namespace Vanrise_Web.Controllers
{
    public class DbTestController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(cs))
            {
                con.Open();
                return Ok("DB Connected");
            }
        }
    }
}
