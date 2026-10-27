using Nomer2;

public class Program
{
    public static void Main()
    {
        RgbColor orange = new RgbColor(255, 165, 0);
        IColorConverter converter = new ColorConverterService();

        Console.WriteLine($"RGB: {orange.R}, {orange.G}, {orange.B}");

        Console.WriteLine($"HEX: {converter.ToHex(orange)}");
        Console.WriteLine($"CMYK: {converter.ToCmyk(orange)}");
        Console.WriteLine($"HSL: {converter.ToHsl(orange)}");
    }
}