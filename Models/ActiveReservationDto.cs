using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vanrise_Web.Models
{
    public class ActiveReservationDto
    {
        public int Id { get; set; }              // ReservationId
        public int PhoneNumberId { get; set; }
        public string PhoneNumber { get; set; }
    }
}