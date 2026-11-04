using Application.Common.Interfaces;
using Application.Services.Abstract;
using Domain.Models;
using System.Net.Mail;

namespace Application.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Category> _categoryRepository;

        public UserService(IRepository<User> userRepository, IRepository<Category> categoryRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public IReadOnlyList<User> GetAll() => _userRepository.GetAll();

        public User? GetById(Guid id) => _userRepository.GetById(id);

        public IReadOnlyList<User> GetSubscribers(Guid categoryId)
        {
            EnsureCategoryExists(categoryId);

            return _userRepository.GetAll().Where(user => user.SubscribedCategoryIds.Contains(categoryId)).ToList();
        }

        public User Add(User user)
        {
            ValidateUser(user);
            EnsureEmailIsUnique(user.Email);

            if (user.Id == Guid.Empty) user.Id = Guid.NewGuid();
            if (_userRepository.GetById(user.Id) is not null) throw new InvalidOperationException($"User with ID {user.Id} already exists.");

            user.SubscribedCategoryIds ??= new List<Guid>();
            ValidateSubscriptions(user.SubscribedCategoryIds);

            return _userRepository.Add(user);
        }

        public User Update(User user)
        {
            ValidateUser(user);

            var existingUser = _userRepository.GetById(user.Id) ?? throw new KeyNotFoundException($"User with ID {user.Id} was not found.");

            EnsureEmailIsUnique(user.Email, user.Id);

            user.SubscribedCategoryIds = existingUser.SubscribedCategoryIds;

            return _userRepository.Update(user);
        }

        public void Delete(Guid id)
        {
            if (_userRepository.GetById(id) is null) throw new KeyNotFoundException($"User with ID {id} was not found.");

            _userRepository.Delete(id);
        }

        public void Subscribe(Guid userId, Guid categoryId)
        {
            var user = GetUser(userId);
            EnsureCategoryExists(categoryId);

            if (!user.SubscribedCategoryIds.Contains(categoryId)) user.SubscribedCategoryIds.Add(categoryId);
        }

        public void Unsubscribe(Guid userId, Guid categoryId)
        {
            var user = GetUser(userId);
            EnsureCategoryExists(categoryId);
            user.SubscribedCategoryIds.Remove(categoryId);
        }

        private User GetUser(Guid userId)
        {
            return _userRepository.GetById(userId) ?? throw new KeyNotFoundException($"User with ID {userId} was not found.");
        }

        private void EnsureCategoryExists(Guid categoryId)
        {
            if (_categoryRepository.GetById(categoryId) is null) throw new KeyNotFoundException($"Category with ID {categoryId} was not found.");
        }

        private void ValidateSubscriptions(IEnumerable<Guid> categoryIds)
        {
            foreach (Guid categoryId in categoryIds) EnsureCategoryExists(categoryId);
        }

        private void EnsureEmailIsUnique(string email, Guid? excludedUserId = null)
        {
            bool alreadyExists = _userRepository.GetAll().Any(user => user.Id != excludedUserId &&
                string.Equals(user.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));

            if (alreadyExists) throw new InvalidOperationException($"A user with email '{email}' is already registered.");
        }

        private static void ValidateUser(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrWhiteSpace(user.Username)) throw new ArgumentException("User name cannot be empty.", nameof(user));
            if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("Email cannot be empty.", nameof(user));

            try { _ = new MailAddress(user.Email); }
            catch (FormatException) { throw new ArgumentException("Email format is invalid.", nameof(user)); }
        }
    }
}
