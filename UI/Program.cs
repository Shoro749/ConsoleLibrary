using Application.Common.Interfaces;
using Application.Services;
using Application.Services.Abstract;
using Application.Services.Implementation;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace UI;

internal static class Program
{
    private static void Main()
    {
        try
        {
            using ServiceProvider serviceProvider = ConfigureServices().BuildServiceProvider();
            serviceProvider.GetRequiredService<INotificationService>();
            serviceProvider.GetRequiredService<ConsoleMenu>().Run();
        }
        catch (Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Критична помилка запуску: {exception.Message}");
            Console.ResetColor();
        }
    }

    private static ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

        services.AddSingleton<IBookService, BookService>();
        services.AddSingleton<ICategoryService, CategoryService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IEmailService, ConsoleEmailService>();
        services.AddSingleton<INotificationService, NotificationService>();

        services.AddSingleton<ConsoleMenu>();

        return services;
    }
}
