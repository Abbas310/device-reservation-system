using System.Collections.Generic;
using System.Web.Http;
using Vanrise_Web.Data;
using Vanrise_Web.Models;

namespace Vanrise_Web.Controllers
{
    public class PhoneNumberReservationsController : ApiController
    {
        private readonly PhoneNumberReservationRepository _repo =
            new PhoneNumberReservationRepository();

        // GET /api/phonenumberreservations
        // GET /api/phonenumberreservations?clientId=1&phoneNumberId=2
        [HttpGet]
        public IEnumerable<PhoneNumberReservationDto> Get(int? clientId = null, int? phoneNumberId = null)
        {
            if (clientId == null && phoneNumberId == null)
                return _repo.GetAll();

            return _repo.GetFiltered(clientId, phoneNumberId);
        }

        // GET /api/phonenumberreservations/active?clientId=1
        [HttpGet]
        [Route("api/phonenumberreservations/active")]
        public IHttpActionResult Active(int clientId)
        {
            return Ok(_repo.GetActiveByClient(clientId));
        }

        // POST /api/phonenumberreservations/reserve
        [HttpPost]
        [Route("api/phonenumberreservations/reserve")]
        public IHttpActionResult Reserve(ReserveRequest req)
        {
            if (req == null || req.ClientId <= 0 || req.PhoneNumberId <= 0)
                return BadRequest("ClientId and PhoneNumberId required");

            var newId = _repo.Reserve(req.ClientId, req.PhoneNumberId);
            return Ok(new { Id = newId });
        }

        // POST /api/phonenumberreservations/unreserve
        [HttpPost]
        [Route("api/phonenumberreservations/unreserve")]
        public IHttpActionResult Unreserve(UnreserveRequest req)
        {
            if (req == null || req.ReservationId <= 0)
                return BadRequest("ReservationId required");

            if (!_repo.Unreserve(req.ReservationId))
                return NotFound();

            return Ok();
        }
    }

    // Small request DTOs
    public class ReserveRequest
    {
        public int ClientId { get; set; }
        public int PhoneNumberId { get; set; }
    }

    public class UnreserveRequest
    {
        public int ReservationId { get; set; }
    }
}