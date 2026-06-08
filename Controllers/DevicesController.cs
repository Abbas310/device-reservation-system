using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise_Web.Data;
using Vanrise_Web.Models;
using Vanrise_Web.Data;
using Vanrise_Web.Models;

namespace Vanrise_Web.Controllers
{
    public class DevicesController : ApiController
    {
        private readonly DeviceRepository _repo = new DeviceRepository();

        
        [HttpGet]
        public IEnumerable<DeviceDto> Get()
        {
            return _repo.GetAll();
        }

      
        [HttpGet]
        public IEnumerable<DeviceDto> Get(string search)
        {
            return _repo.GetFiltered(search);
        }

       
        [HttpPost]
        public IHttpActionResult Post(DeviceDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name required");

            var device = _repo.Add(dto.Name.Trim());
            return Ok(device);
        }

        
        [HttpPut]
        public IHttpActionResult Put(int id, DeviceDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name required");

            var ok = _repo.Update(id, dto.Name.Trim());
            if (!ok) return NotFound();

            return Ok();
        }

        
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
                return BadRequest("Cannot delete this device because it is used by existing phone numbers (and/or reservations).");
            }
        }
    }
}
