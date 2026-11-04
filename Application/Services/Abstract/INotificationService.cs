using Domain.Models;

namespace Application.Services.Abstract
{
    public interface INotificationService
    {
        void Notify(Book book);
    }
}
