namespace UDPServer.Models
{
    public class Person
    {
        // Id будет автоматически генерироваться БД как первичный ключ (автоинкремент).
        public int Id { get; set; }

        // Имя человека. Значение по умолчанию - пустая строка, чтобы избежать null.
        public string Name { get; set; } = string.Empty;

        // Должность.
        public string Post { get; set; } = string.Empty;

        // Баланс (денежная сумма). Используем decimal для точности.
        public decimal Balance { get; set; }

        // Очки мощи (целое число).
        public int PowerPoints { get; set; }
    } 
}