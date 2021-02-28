using Orders.Domain.Entities.Client;
using Orders.Domain.Entities.DTO;
using Orders.Domain.Enums;

namespace Orders.Domain.Entities
{
    public class Order : Entity
    {
        public ProductDto Product { get; set; }
        public Status Status { get; set; }
        public User User { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }

        public bool IsValid()
        {
            return User != null &&
                !string.IsNullOrEmpty(User.Name) &&
                !string.IsNullOrEmpty(User.Email) &&
                !string.IsNullOrEmpty(User.Phone) &&
                Product != null;
        }
    }
}
