using System.Net;
using System.Net.Sockets;
using System.Text;

// Создаём UDP-сокет
using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

// Адрес сервера (localhost, порт 5555)
EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);

Console.WriteLine("UDP-клиент. Команды:");
Console.WriteLine("  add   - добавить запись (ввод полей по одному)");
Console.WriteLine("  list  - показать все записи");
Console.WriteLine("  exit  - выход");

// Бесконечный цикл обработки команд
while (true)
{
    Console.Write("\n :) ---> ");
    string command = Console.ReadLine() ?? "";
    if (string.IsNullOrEmpty(command))
    {
        Console.WriteLine("только предложенные команды");
        continue;
    }

    switch (command)
    {
        case "exit":
            return;

        case "list":
            byte[] data = Encoding.UTF8.GetBytes("LIST");
            await udpSocket.SendToAsync(data, serverEP);
            Console.WriteLine("Запрос списка отправлен, ожидаю ответ...");

            byte[] buffer = new byte[8192];
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                var receive = await udpSocket.ReceiveFromAsync(buffer, remoteEP);
                string response = Encoding.UTF8.GetString(buffer, 0, receive.ReceivedBytes);
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка приёма: {ex.Message}");
            }
            break;

        case "add":
            Console.Write("Введите Name: ");
            string name = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Имя не может быть пустым.");
                break;
            }

            Console.Write("Введите Post: ");
            string post = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(post))
            {
                Console.WriteLine("Должность не может быть пустой.");
                break;
            }

            Console.Write("Введите Balance (число): ");
            string balanceStr = Console.ReadLine() ?? "";
            if (!decimal.TryParse(balanceStr, out decimal balance))
            {
                Console.WriteLine("Некорректное число для Balance.");
                break;
            }

            Console.Write("Введите Power Points (целое число): ");
            string ppStr = Console.ReadLine() ?? "";
            if (!int.TryParse(ppStr, out int powerPoints))
            {
                Console.WriteLine("Некорректное целое число для Power Points.");
                break;
            }

            string message = $"{name} {post} {balance} {powerPoints}";
            data = Encoding.UTF8.GetBytes(message);
            await udpSocket.SendToAsync(data, serverEP);
            Console.WriteLine("Данные отправлены, ожидается ответ...");

            buffer = new byte[1024];
            remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                var result = await udpSocket.ReceiveFromAsync(buffer, remoteEP);
                string response = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка приёма: {ex.Message}");
            }
            break;

        default:
            Console.WriteLine("Неизвестная команда. Используйте add, list или exit.");
            break;
    }
}