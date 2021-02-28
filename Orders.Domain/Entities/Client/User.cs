using System.Collections.Generic;

namespace Orders.Domain.Entities.Client
{
    public class User : Entity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public IEnumerable<Characters> Characters { get; set; }
    }
}
