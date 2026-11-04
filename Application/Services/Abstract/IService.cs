namespace Application.Services.Abstract
{
    public interface IService<T>
    {
        IReadOnlyList<T> GetAll();
        T? GetById(Guid id);
        T Add(T item);
        T Update(T item);
        void Delete(Guid id);
    }
}
