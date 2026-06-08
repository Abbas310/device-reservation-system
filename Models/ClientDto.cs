using System;

namespace Vanrise_Web.Models
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        
        public int Type { get; set; }

        
        public DateTime? BirthDate { get; set; }

        
        public string BirthDateText { get; set; }
    }
}
