using System;

namespace Vanrise_Web.Models
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // 1 = Individual, 2 = Organization
        public int Type { get; set; }

        // Stored in DB (DATE)
        public DateTime? BirthDate { get; set; }

        // Sent from UI as "yyyy-MM-dd"
        public string BirthDateText { get; set; }
    }
}
