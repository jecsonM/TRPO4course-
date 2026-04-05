using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.Services;
using WpfAppWorkStations.Services;
internal class Program
{
    private static void Main(string[] args)
    {
        IDBWorkStationsService dBWorkStationsService = new DBWorkStationsService();

        Console.WriteLine("Введите логин для директора: ");
        string login = Console.ReadLine();

        Console.WriteLine("Введите Пароль для директора: ");
        string password = Console.ReadLine();


        
    }
}