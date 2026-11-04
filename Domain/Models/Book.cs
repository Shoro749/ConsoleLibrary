using Domain.Common;

namespace Domain.Models
{
    public class Book : IEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Pages { get; set; }

        public Guid CategoryId { get; set; }
    }
}
