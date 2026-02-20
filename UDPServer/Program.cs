using System.Net;               // Пространство имён для IPEndPoint, IPAddress
using System.Net.Sockets;       // Пространство имён для Socket
using System.Text;              // Для Encoding (преобразование строк в байты и обратно)
using UDPServer.Models;
using UDPServer.Context;

using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


var localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);


udpSocket.Bind(localEP);         //привязываем соекет

Console.WriteLine("UDP-сервер запущен на порту 5555...");

byte[] buffer = new byte[4096];

EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

while (true)
{
    try
    {

        var result = await udpSocket.ReceiveFromAsync(buffer, remoteEP);
        string received = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);

        Console.WriteLine($"Получено от {result.RemoteEndPoint}: {received}");

        string response = await ProcessRequestAsync(received);
        byte[] responseData = Encoding.UTF8.GetBytes(response);

        await udpSocket.SendToAsync(responseData, result.RemoteEndPoint);
    }
    catch (Exception ex)
    {

        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

async Task<string> ProcessRequestAsync(string request)
{
    if (string.IsNullOrWhiteSpace(request))
        return "Пустой запрос";

    if (request.Trim().ToUpper() == "LIST")
    {
        using var db = new AppDbContext();
        await db.Database.EnsureCreatedAsync();
        var persons = db.Persons.ToList();
        if (persons.Count == 0)
        {
            return "Список пуст";
        }


        var sb = new StringBuilder();

        sb.AppendLine("ID\tName\tPost\tBalance\tPowerPoints");


        foreach (var p in persons)
        {
            sb.AppendLine($"{p.Id}\t{p.Name}\t{p.Post}\t{p.Balance}\t{p.PowerPoints}");
        }

        return sb.ToString();
    }
    else
    {
        var parts = request.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 4)
        {
            return "Ошибка: ожидается 4 поля (Name Post Balance PowerPoints)";
        }


        string name = parts[0];
        string post = parts[1];

        if (!decimal.TryParse(parts[2], out decimal balance))
        {
            return "Ошибка: Balance должен быть числом";
        }

        if (!int.TryParse(parts[3], out int powerPoints))
        {
           return "Ошибка: Power Points должен быть целым числом";
        }


        using var db = new AppDbContext();


        await db.Database.EnsureCreatedAsync();

        var person = new Person
        {
            Name = name,
            Post = post,
            Balance = balance,
            PowerPoints = powerPoints
        };

        // Добавляем объект в контекст.
        await db.Persons.AddAsync(person);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ошибка: {ex}");
        }                        

        return $"Сохранено: {person.Name} (Id={person.Id})";
    }
}