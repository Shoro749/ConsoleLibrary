using Application.Common.Interfaces;
using Application.Services.Abstract;
using Domain.Models;

namespace Application.Services.Implementation
{
    public sealed class NotificationService : INotificationService, IDisposable
    {
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IEmailService _emailService;
        private bool _disposed;

        public NotificationService(
            IBookService bookService,
            IUserService userService,
            ICategoryService categoryService,
            IEmailService emailService)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

            _bookService.BookAdded += OnBookAdded;
        }

        private void OnBookAdded(Book book)
        {
            Notify(book);
        }

        public void Notify(Book book)
        {
            ArgumentNullException.ThrowIfNull(book);

            var category = _categoryService.GetById(book.CategoryId);
            if (category is null) return;

            var subscribers = _userService.GetSubscribers(book.CategoryId);
            string categoryName = category.Name;
            string subject = $"New book in {categoryName}";
            string body = $"A new book has been added to the library:\n" +
                          $"Title: {book.Title}\n" +
                          $"Author: {book.Author}\n" +
                          $"Pages: {book.Pages}\n" +
                          $"Category: {categoryName}";

            foreach (var user in subscribers)
            {
                try
                {
                    _emailService.Send(user.Email, subject, body);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to send notification to {user.Email} for book '{book.Title}' in category '{categoryName}'.");
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _bookService.BookAdded -= OnBookAdded;
            _disposed = true;
        }
    }
}
