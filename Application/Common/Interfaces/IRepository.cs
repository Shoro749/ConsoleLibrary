using Domain.Common;

namespace Application.Common.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        IReadOnlyList<T> GetAll();
        T? GetById(Guid id);
        T Add(T item);
        T Update(T item);
        void Delete(Guid id);
    }
}
