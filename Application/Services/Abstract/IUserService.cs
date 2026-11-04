using Domain.Models;

namespace Application.Services.Abstract
{
    public interface IUserService : IService<User>
    {
        IReadOnlyList<User> GetSubscribers(Guid categoryId);
        void Subscribe(Guid userId, Guid categoryId);
        void Unsubscribe(Guid userId, Guid categoryId);
    }
}
