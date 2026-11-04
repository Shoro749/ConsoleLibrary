using Application.Common.Interfaces;
using Domain.Common;

namespace Infrastructure.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly List<T> _items = new();

        public T Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            _items.Add(item);
            return item;
        }

        public void Delete(Guid id)
        {
            var item = GetById(id);

            if (item == null) throw new KeyNotFoundException($"Item with id {id} not found.");

            _items.Remove(item);
        }

        public IReadOnlyList<T> GetAll() => _items.AsReadOnly();

        public T? GetById(Guid id) => _items.FirstOrDefault(item => item.Id == id);

        public T Update(T item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            var index = _items.FindIndex(i => i.Id == item.Id);

            if (index == -1) throw new KeyNotFoundException($"Item with id {item.Id} not found.");

            _items[index] = item;
            return item;
        }
    }
}
