using Application.Common.Interfaces;
using Application.Services.Abstract;
using Domain.Models;

namespace Application.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<User> _userRepository;

        public CategoryService(
            IRepository<Category> categoryRepository,
            IRepository<Book> bookRepository,
            IRepository<User> userRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public IReadOnlyList<Category> GetAll() => _categoryRepository.GetAll();

        public Category? GetById(Guid id) => _categoryRepository.GetById(id);

        public Category Add(Category category)
        {
            ValidateName(category);
            category.Name = category.Name.Trim();
            EnsureNameIsUnique(category.Name);

            if (category.Id == Guid.Empty) category.Id = Guid.NewGuid();
            if (_categoryRepository.GetById(category.Id) is not null) throw new InvalidOperationException($"Category with ID {category.Id} already exists.");

            return _categoryRepository.Add(category);
        }

        public Category Update(Category category)
        {
            ValidateName(category);

            if (_categoryRepository.GetById(category.Id) is null) throw new KeyNotFoundException($"Category with ID {category.Id} was not found.");

            category.Name = category.Name.Trim();
            EnsureNameIsUnique(category.Name, category.Id);
            return _categoryRepository.Update(category);
        }

        public void Delete(Guid id)
        {
            if (_categoryRepository.GetById(id) is null) throw new KeyNotFoundException($"Category with ID {id} was not found.");

            bool containsBooks = _bookRepository.GetAll().Any(book => book.CategoryId == id);

            if (containsBooks) throw new InvalidOperationException("Cannot delete a category that still contains books.");

            bool hasSubscribers = _userRepository.GetAll().Any(user => user.SubscribedCategoryIds.Contains(id));

            if (hasSubscribers) throw new InvalidOperationException("Cannot delete a category that still has subscribers.");

            _categoryRepository.Delete(id);
        }

        private static void ValidateName(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            if (string.IsNullOrWhiteSpace(category.Name)) throw new ArgumentException("Category name cannot be empty.", nameof(category));
        }

        private void EnsureNameIsUnique(string name, Guid? excludedId = null)
        {
            bool alreadyExists = _categoryRepository.GetAll().Any(category => category.Id != excludedId &&
                string.Equals(category.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));

            if (alreadyExists) throw new InvalidOperationException($"Category '{name}' already exists.");
        }
    }
}
