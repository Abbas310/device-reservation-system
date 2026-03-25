using System;
using System.Data.SqlClient;
using System.Net;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using Vanrise_Web.Data;
using Vanrise_Web.Data;
using Vanrise_Web.Models;

namespace Vanrise_Web.Controllers
{
    public class ClientsController : ApiController
    {
        private readonly ClientRepository _repo = new ClientRepository();

        // GET /api/clients
        // GET /api/clients?search=a&type=1
        [HttpGet]
        public IEnumerable<ClientDto> Get(string search = null, int? type = null)
        {
            if (string.IsNullOrWhiteSpace(search) && type == null)
                return _repo.GetAll();

            return _repo.GetFiltered(search, type);
        }

        // POST /api/clients
        [HttpPost]
        public IHttpActionResult Post(ClientDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name required");

            dto.Name = dto.Name.Trim();
            dto.BirthDate = ParseBirthDate(dto);

            return Ok(_repo.Add(dto));
        }

        // PUT /api/clients/5
        [HttpPut]
        public IHttpActionResult Put(int id, ClientDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name required");

            dto.Id = id;
            dto.Name = dto.Name.Trim();
            dto.BirthDate = ParseBirthDate(dto);

            if (!_repo.Update(dto)) return NotFound();
            return Ok();
        }

        // DELETE /api/clients/5
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

        private static DateTime? ParseBirthDate(ClientDto dto)
        {
            // BirthDate only for Individual
            if (dto.Type != 1) return null;

            // If model binding already populated it, keep it
            if (dto.BirthDate.HasValue) return dto.BirthDate;

            // Otherwise parse from text yyyy-MM-dd
            if (string.IsNullOrWhiteSpace(dto.BirthDateText)) return null;

            if (DateTime.TryParseExact(
                dto.BirthDateText.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var d))
            {
                return d;
            }

            return null;
        }
    }
}
