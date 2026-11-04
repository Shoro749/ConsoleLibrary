using Application.Services.Abstract;
using Domain.Models;

namespace UI;

public sealed class ConsoleMenu
{
    private readonly IUserService _userService;
    private readonly IBookService _bookService;
    private readonly ICategoryService _categoryService;

    public ConsoleMenu(
        IUserService userService,
        IBookService bookService,
        ICategoryService categoryService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            ShowHeader("КЕРУВАННЯ БІБЛІОТЕКОЮ");
            Console.WriteLine("1. Управління користувачами");
            Console.WriteLine("2. Управління категоріями");
            Console.WriteLine("3. Управління книгами");
            Console.WriteLine("4. Підписки на категорії");
            Console.WriteLine("0. Вихід");

            switch (ReadChoice("Оберіть пункт: ", "0", "1", "2", "3", "4"))
            {
                case "1": RunUsersMenu(); break;
                case "2": RunCategoriesMenu(); break;
                case "3": RunBooksMenu(); break;
                case "4": RunSubscriptionsMenu(); break;
                case "0": running = false; break;
            }
        }

        Console.WriteLine("До побачення!");
    }

    private void RunUsersMenu()
    {
        bool back = false;
        while (!back)
        {
            ShowHeader("УПРАВЛІННЯ КОРИСТУВАЧАМИ");
            Console.WriteLine("1. Реєстрація нового користувача");
            Console.WriteLine("2. Список усіх користувачів");
            Console.WriteLine("3. Редагувати дані користувача");
            Console.WriteLine("4. Видалити користувача");
            Console.WriteLine("0. Назад");

            switch (ReadChoice("Оберіть пункт: ", "0", "1", "2", "3", "4"))
            {
                case "1": Execute(RegisterUser); break;
                case "2": Execute(ListUsers); break;
                case "3": Execute(EditUser); break;
                case "4": Execute(DeleteUser); break;
                case "0": back = true; break;
            }
        }
    }

    private void RunCategoriesMenu()
    {
        bool back = false;
        while (!back)
        {
            ShowHeader("УПРАВЛІННЯ КАТЕГОРІЯМИ");
            Console.WriteLine("1. Додати категорію");
            Console.WriteLine("2. Список категорій");
            Console.WriteLine("3. Редагувати категорію");
            Console.WriteLine("4. Видалити категорію");
            Console.WriteLine("0. Назад");

            switch (ReadChoice("Оберіть пункт: ", "0", "1", "2", "3", "4"))
            {
                case "1": Execute(AddCategory); break;
                case "2": Execute(ListCategories); break;
                case "3": Execute(EditCategory); break;
                case "4": Execute(DeleteCategory); break;
                case "0": back = true; break;
            }
        }
    }

    private void RunBooksMenu()
    {
        bool back = false;
        while (!back)
        {
            ShowHeader("УПРАВЛІННЯ КНИГАМИ");
            Console.WriteLine("1. Додати книгу");
            Console.WriteLine("2. Список усіх книг");
            Console.WriteLine("3. Редагувати книгу");
            Console.WriteLine("4. Видалити книгу");
            Console.WriteLine("0. Назад");

            switch (ReadChoice("Оберіть пункт: ", "0", "1", "2", "3", "4"))
            {
                case "1": Execute(AddBook); break;
                case "2": Execute(ListBooks); break;
                case "3": Execute(EditBook); break;
                case "4": Execute(DeleteBook); break;
                case "0": back = true; break;
            }
        }
    }

    private void RunSubscriptionsMenu()
    {
        bool back = false;
        while (!back)
        {
            ShowHeader("ПІДПИСКИ НА КАТЕГОРІЇ");
            Console.WriteLine("1. Підписати користувача на категорію");
            Console.WriteLine("2. Відписати користувача від категорії");
            Console.WriteLine("3. Переглянути підписки користувача");
            Console.WriteLine("0. Назад");

            switch (ReadChoice("Оберіть пункт: ", "0", "1", "2", "3"))
            {
                case "1": Execute(SubscribeUser); break;
                case "2": Execute(UnsubscribeUser); break;
                case "3": Execute(ListUserSubscriptions); break;
                case "0": back = true; break;
            }
        }
    }

    private void RegisterUser()
    {
        ShowHeader("РЕЄСТРАЦІЯ КОРИСТУВАЧА");
        var user = new User
        {
            Username = ReadRequired("Ім'я: "),
            Email = ReadRequired("Email: ")
        };

        _userService.Add(user);
        ShowSuccess("Користувача успішно зареєстровано.");
    }

    private void ListUsers()
    {
        ShowHeader("СПИСОК КОРИСТУВАЧІВ");
        var users = _userService.GetAll();
        if (users.Count == 0)
        {
            ShowInfo("Користувачів ще немає.");
            return;
        }

        for (int i = 0; i < users.Count; i++)
        {
            User user = users[i];
            Console.WriteLine($"{i + 1}. {user.Username} | {user.Email} | ID: {ShortId(user.Id)}");
        }

        Pause();
    }

    private void EditUser()
    {
        User? user = SelectUser("Оберіть користувача для редагування: ");
        if (user is null) return;

        string username = ReadWithDefault("Нове ім'я", user.Username);
        string email = ReadWithDefault("Новий email", user.Email);
        _userService.Update(new User { Id = user.Id, Username = username, Email = email });
        ShowSuccess("Дані користувача оновлено.");
    }

    private void DeleteUser()
    {
        User? user = SelectUser("Оберіть користувача для видалення: ");
        if (user is null || !Confirm("Підтвердити видалення?")) return;

        _userService.Delete(user.Id);
        ShowSuccess("Користувача видалено.");
    }

    private void AddCategory()
    {
        ShowHeader("ДОДАВАННЯ КАТЕГОРІЇ");
        _categoryService.Add(new Category { Name = ReadRequired("Назва категорії: ") });
        ShowSuccess("Категорію додано.");
    }

    private void ListCategories()
    {
        ShowHeader("СПИСОК КАТЕГОРІЙ");
        var categories = _categoryService.GetAll();
        if (categories.Count == 0)
        {
            ShowInfo("Категорій ще немає.");
            return;
        }

        for (int i = 0; i < categories.Count; i++)
        {
            Category category = categories[i];
            int bookCount = _bookService.GetAll().Count(book => book.CategoryId == category.Id);
            Console.WriteLine($"{i + 1}. {category.Name} | Книг: {bookCount} | ID: {ShortId(category.Id)}");
        }

        Pause();
    }

    private void EditCategory()
    {
        Category? category = SelectCategory("Оберіть категорію для редагування: ");
        if (category is null) return;

        string name = ReadWithDefault("Нова назва", category.Name);
        _categoryService.Update(new Category { Id = category.Id, Name = name });
        ShowSuccess("Категорію оновлено.");
    }

    private void DeleteCategory()
    {
        Category? category = SelectCategory("Оберіть категорію для видалення: ");
        if (category is null || !Confirm("Підтвердити видалення?")) return;

        _categoryService.Delete(category.Id);
        ShowSuccess("Категорію видалено.");
    }

    private void AddBook()
    {
        ShowHeader("ДОДАВАННЯ КНИГИ");
        Category? category = SelectCategory("Оберіть категорію книги: ");
        if (category is null) return;

        var book = new Book
        {
            Title = ReadRequired("Назва: "),
            Author = ReadRequired("Автор: "),
            Pages = ReadPositiveInt("Кількість сторінок: "),
            CategoryId = category.Id
        };

        _bookService.Add(book);
        ShowSuccess("Книгу успішно додано до каталогу.");
    }

    private void ListBooks()
    {
        ShowHeader("СПИСОК КНИГ");
        var books = _bookService.GetAll();
        if (books.Count == 0)
        {
            ShowInfo("Книг ще немає.");
            return;
        }

        for (int i = 0; i < books.Count; i++)
        {
            Book book = books[i];
            string categoryName = _categoryService.GetById(book.CategoryId)?.Name ?? "невідома";
            Console.WriteLine($"{i + 1}. {book.Title} — {book.Author} | {book.Pages} стор. | Категорія: {categoryName}");
            Console.WriteLine($"   ID: {ShortId(book.Id)}");
        }

        Pause();
    }

    private void EditBook()
    {
        Book? book = SelectBook("Оберіть книгу для редагування: ");
        if (book is null) return;
        Category? category = SelectCategory("Оберіть нову категорію: ");
        if (category is null) return;

        var updatedBook = new Book
        {
            Id = book.Id,
            Title = ReadWithDefault("Нова назва", book.Title),
            Author = ReadWithDefault("Новий автор", book.Author),
            Pages = ReadPositiveIntWithDefault("Нова кількість сторінок", book.Pages),
            CategoryId = category.Id
        };

        _bookService.Update(updatedBook);
        ShowSuccess("Книгу оновлено.");
    }

    private void DeleteBook()
    {
        Book? book = SelectBook("Оберіть книгу для видалення: ");
        if (book is null || !Confirm("Підтвердити видалення?")) return;

        _bookService.Delete(book.Id);
        ShowSuccess("Книгу видалено.");
    }

    private void SubscribeUser()
    {
        User? user = SelectUser("Оберіть користувача: ");
        if (user is null) return;
        Category? category = SelectCategory("Оберіть категорію: ");
        if (category is null) return;

        _userService.Subscribe(user.Id, category.Id);
        ShowSuccess("Користувача підписано на категорію.");
    }

    private void UnsubscribeUser()
    {
        User? user = SelectUser("Оберіть користувача: ");
        if (user is null) return;
        Category? category = SelectCategory("Оберіть категорію: ");
        if (category is null) return;

        _userService.Unsubscribe(user.Id, category.Id);
        ShowSuccess("Користувача відписано від категорії.");
    }

    private void ListUserSubscriptions()
    {
        User? user = SelectUser("Оберіть користувача: ");
        if (user is null) return;

        ShowHeader($"ПІДПИСКИ: {user.Username}");
        var subscriptions = user.SubscribedCategoryIds
            .Select(id => _categoryService.GetById(id))
            .Where(category => category is not null)
            .Select(category => category!.Name)
            .ToList();

        if (subscriptions.Count == 0) ShowInfo("Користувач не має активних підписок.");
        else foreach (string subscription in subscriptions) Console.WriteLine($"- {subscription}");
        Pause();
    }

    private User? SelectUser(string prompt)
    {
        var users = _userService.GetAll();
        if (users.Count == 0)
        {
            ShowInfo("Користувачів ще немає.");
            return null;
        }

        ShowHeader("КОРИСТУВАЧІ");
        for (int i = 0; i < users.Count; i++) Console.WriteLine($"{i + 1}. {users[i].Username} | {users[i].Email}");
        int index = ReadIndex(prompt, users.Count);
        return users[index];
    }

    private Category? SelectCategory(string prompt)
    {
        var categories = _categoryService.GetAll();
        if (categories.Count == 0)
        {
            ShowInfo("Категорій ще немає.");
            return null;
        }

        ShowHeader("КАТЕГОРІЇ");
        for (int i = 0; i < categories.Count; i++) Console.WriteLine($"{i + 1}. {categories[i].Name}");
        int index = ReadIndex(prompt, categories.Count);
        return categories[index];
    }

    private Book? SelectBook(string prompt)
    {
        var books = _bookService.GetAll();
        if (books.Count == 0)
        {
            ShowInfo("Книг ще немає.");
            return null;
        }

        ShowHeader("КНИГИ");
        for (int i = 0; i < books.Count; i++) Console.WriteLine($"{i + 1}. {books[i].Title} — {books[i].Author}");
        int index = ReadIndex(prompt, books.Count);
        return books[index];
    }

    private static int ReadIndex(string prompt, int count)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= count) return index - 1;
            ShowError($"Введіть число від 1 до {count}.");
        }
    }

    private static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value > 0) return value;
            ShowError("Введіть додатне ціле число.");
        }
    }

    private static int ReadPositiveIntWithDefault(string prompt, int currentValue)
    {
        while (true)
        {
            Console.Write($"{prompt} [{currentValue}]: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return currentValue;
            if (int.TryParse(input, out int value) && value > 0) return value;
            ShowError("Введіть додатне ціле число або натисніть Enter для збереження поточного.");
        }
    }

    private static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string value = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(value)) return value;
            ShowError("Поле не може бути порожнім.");
        }
    }

    private static string ReadWithDefault(string prompt, string currentValue)
    {
        Console.Write($"{prompt} [{currentValue}]: ");
        string? value = Console.ReadLine()?.Trim();
        return string.IsNullOrWhiteSpace(value) ? currentValue : value;
    }

    private static string ReadChoice(string prompt, params string[] choices)
    {
        while (true)
        {
            Console.Write(prompt);
            string choice = Console.ReadLine()?.Trim() ?? string.Empty;
            if (choices.Contains(choice, StringComparer.OrdinalIgnoreCase)) return choice;
            ShowError("Невірний пункт меню. Спробуйте ще раз.");
        }
    }

    private static bool Confirm(string prompt)
    {
        string answer = ReadChoice($"{prompt} (т/н): ", "т", "н", "y", "n");
        return answer.Equals("т", StringComparison.OrdinalIgnoreCase) || answer.Equals("y", StringComparison.OrdinalIgnoreCase);
    }

    private static void Execute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or KeyNotFoundException)
        {
            ShowError(exception.Message);
            Pause();
        }
    }

    private static void ShowHeader(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(new string('═', 64));
        Console.WriteLine($"{title.PadLeft((64 + title.Length) / 2).PadRight(64)}");
        Console.WriteLine(new string('═', 64));
        Console.ResetColor();
    }

    private static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✓ {message}");
        Console.ResetColor();
        Pause();
    }

    private static void ShowInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n{message}");
        Console.ResetColor();
        Pause();
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nПомилка: {message}");
        Console.ResetColor();
    }

    private static void Pause()
    {
        Console.WriteLine("\nНатисніть Enter, щоб продовжити...");
        Console.ReadLine();
    }

    private static string ShortId(Guid id) => id.ToString("N")[..8];
}
