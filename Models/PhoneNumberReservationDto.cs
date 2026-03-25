using System;

namespace Vanrise_Web.Models
{
    public class PhoneNumberReservationDto
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; }

        public int PhoneNumberId { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
