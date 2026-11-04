using Application.Common.Interfaces;

namespace UI;

public sealed class ConsoleEmailService : IEmailService
{
    public void Send(string recipient, string subject, string body)
    {
        ConsoleColor previousColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    EMAIL NOTIFICATION                    ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║ To: {recipient}");
            Console.WriteLine($"║ Subject: {subject}");
            Console.WriteLine("╟────────────────────────────────────────────────────────────╢");
            foreach (string line in body.Split('\n'))
            {
                Console.WriteLine($"║ {line.TrimEnd('\r')}");
            }

            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        }
        finally
        {
            Console.ForegroundColor = previousColor;
        }
    }
}
