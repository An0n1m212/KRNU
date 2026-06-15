public struct RgbColor
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }

    public RgbColor(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
}

public struct CmykColor
{
    public double C, M, Y, K;
    public override string ToString() => $"C:{C:P0}, M:{M:P0}, Y:{Y:P0}, K:{K:P0}";
}

public struct HslColor
{
    public double H, S, L;
    public override string ToString() => $"H:{H:F0}°, S:{S:P0}, L:{L:P0}";
}