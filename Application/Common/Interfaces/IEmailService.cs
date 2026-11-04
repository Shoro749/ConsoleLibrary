namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        void Send(string recipient, string subject, string body);
    }
}
