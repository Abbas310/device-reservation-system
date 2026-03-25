using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Web.Http;
using Vanrise_Web.Data;
using Vanrise_Web.Data;
using Vanrise_Web.Models;
using Vanrise_Web.Models;

namespace Vanrise_Web.Controllers
{
    public class PhoneNumbersController : ApiController
    {
        private readonly PhoneNumberRepository _repo = new PhoneNumberRepository();

        // GET /api/phonenumbers
        // GET /api/phonenumbers?number=70&deviceId=1
        [HttpGet]
        public IEnumerable<PhoneNumberDto> Get(string number = null, int? deviceId = null)
        {
            if (string.IsNullOrWhiteSpace(number) && deviceId == null)
                return _repo.GetAll();

            return _repo.GetFiltered(number, deviceId);
        }

        // POST /api/phonenumbers   body: { "Number":"70123456", "DeviceId":1 }
        [HttpPost]
        public IHttpActionResult Post(PhoneNumberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Number))
                return BadRequest("Number required");
            if (dto.DeviceId <= 0)
                return BadRequest("DeviceId required");

            var created = _repo.Add(dto.Number.Trim(), dto.DeviceId);
            return Ok(created);
        }

        // PUT /api/phonenumbers/5
        [HttpPut]
        public IHttpActionResult Put(int id, PhoneNumberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Number))
                return BadRequest("Number required");
            if (dto.DeviceId <= 0)
                return BadRequest("DeviceId required");

            var ok = _repo.Update(id, dto.Number.Trim(), dto.DeviceId);
            if (!ok) return NotFound();

            return Ok();
        }

        // DELETE /api/phonenumbers/5
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                if (!_repo.Delete(id)) return NotFound();
                return Ok();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Content(HttpStatusCode.Conflict, new { Message = "Cannot delete this phone number because it is used by an existing reservation." });
            }
        }
        // GET /api/phonenumbers/available
        [HttpGet]
        [Route("api/phonenumbers/available")]
        public IHttpActionResult Available()
        {
            return Ok(_repo.GetAvailable());
        }
    }
}
