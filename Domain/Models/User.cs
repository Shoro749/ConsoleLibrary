using Domain.Common;

namespace Domain.Models
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public ICollection<Guid> SubscribedCategoryIds { get; set; } = new List<Guid>();
    }
}
