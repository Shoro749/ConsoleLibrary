using Domain.Models;

namespace Application.Services.Abstract
{
    public interface IBookService : IService<Book>
    {
        event Action<Book>? BookAdded;
        IReadOnlyList<Book> GetByCategory(Guid categoryId);
    }
}
