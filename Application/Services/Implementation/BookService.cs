using Application.Common.Interfaces;
using Application.Services.Abstract;
using Domain.Models;

namespace Application.Services.Implementation;

public class BookService : IBookService
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<Category> _categoryRepository;

    public event Action<Book>? BookAdded;

    public BookService(IRepository<Book> bookRepository, IRepository<Category> categoryRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public IReadOnlyList<Book> GetAll() => _bookRepository.GetAll();

    public IReadOnlyList<Book> GetByCategory(Guid categoryId)
    {
        EnsureCategoryExists(categoryId);
        return _bookRepository.GetAll().Where(book => book.CategoryId == categoryId).ToList();
    }

    public Book? GetById(Guid id) => _bookRepository.GetById(id);

    public Book Add(Book book)
    {
        ValidateBook(book);
        EnsureCategoryExists(book.CategoryId);

        if (book.Id == Guid.Empty) book.Id = Guid.NewGuid();

        if (_bookRepository.GetById(book.Id) is not null) throw new InvalidOperationException($"Book with ID {book.Id} already exists.");

        var addedBook = _bookRepository.Add(book);

        BookAdded?.Invoke(addedBook);

        return addedBook;
    }

    public Book Update(Book book)
    {
        ValidateBook(book);

        if (_bookRepository.GetById(book.Id) is null) throw new KeyNotFoundException($"Book with ID {book.Id} was not found.");

        return _bookRepository.Update(book);
    }

    public void Delete(Guid id)
    {
        if (_bookRepository.GetById(id) is null) throw new KeyNotFoundException($"Book with ID {id} was not found.");

        _bookRepository.Delete(id);
    }

    private void ValidateBook(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        if (string.IsNullOrWhiteSpace(book.Title)) throw new ArgumentException("Book title cannot be empty.", nameof(book));
        if (string.IsNullOrWhiteSpace(book.Author)) throw new ArgumentException("Book author cannot be empty.", nameof(book));
        if (book.Pages <= 0) throw new ArgumentException("Book must have at least one page.", nameof(book));
    }

    private void EnsureCategoryExists(Guid categoryId) => _ = GetCategory(categoryId);

    private Category GetCategory(Guid categoryId)
    {
        return _categoryRepository.GetById(categoryId) ?? throw new KeyNotFoundException($"Category with ID {categoryId} was not found.");
    }
}
