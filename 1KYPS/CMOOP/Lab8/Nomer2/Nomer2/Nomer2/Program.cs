using Nomer2.Class.Auxiliary;
using Nomer2.Class.Landings;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Registry myAeroClub = new Registry();

        myAeroClub.AddDevice(new Plane { Name = "Boeing 747", HasEngine = true, IsElectronic = true, Power = 50000 });
        myAeroClub.AddDevice(new HotAirBalloon { Name = "Zodiac", HasEngine = false, IsElectronic = false, Material = "Nylon" });
        myAeroClub.AddDevice(new FlyingCarpet { Name = "Persian Magic", HasEngine = false, IsElectronic = false });



        Plane p1 = new Plane { Name = "Cessna", Power = 300 };
        Plane p2 = (Plane)p1.Clone();
        p2.Name = "Cessna Copy";
        myAeroClub.AddDevice(p1);
        myAeroClub.AddDevice(p2);

        myAeroClub.ShowAll();

        myAeroClub.SortByName();
        Console.WriteLine("\n(Після сортування за назвою)");
        myAeroClub.ShowAll();

        myAeroClub.ShowWithoutEngine();
    }
}