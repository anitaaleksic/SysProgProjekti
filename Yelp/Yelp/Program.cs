using Yelp;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Unestie lokaciju ");
        string lokacija = Console.ReadLine() ?? String.Empty;
        Console.WriteLine("Unesite minimalni rejting ");
        double rating = int.Parse(Console.ReadLine() ?? "0");
        Console.WriteLine("Unesite kategoriju: ");
        string kategorija = Console.ReadLine() ?? String.Empty;
        Console.WriteLine();
        var observer = new LokalObserver();
        var server = new HttpServerObservable();
        server.Subscribe(observer);
        server.Pretraga(lokacija, rating, kategorija);
        Console.ReadLine();
    }
}