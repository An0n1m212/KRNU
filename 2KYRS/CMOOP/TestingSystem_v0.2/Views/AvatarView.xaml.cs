using System.Windows.Controls;
using System.Windows.Media;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Круглый аватар. Если файла нет — аватар по умолчанию: цветной круг с инициалами
/// (цвет зависит от Id пользователя).
/// </summary>
public partial class AvatarView : UserControl
{
    private static readonly Color[] Palette =
    {
        Color.FromRgb(0x00, 0x67, 0xC0), Color.FromRgb(0x2E, 0x7D, 0x32), Color.FromRgb(0x8E, 0x24, 0xAA),
        Color.FromRgb(0xD8, 0x43, 0x15), Color.FromRgb(0x00, 0x83, 0x8F), Color.FromRgb(0xC2, 0x18, 0x5B)
    };

    public AvatarView() => InitializeComponent();

    public void Show(int userId, string fullName, string? avatarPath)
    {
        var words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        InitialsText.Text = string.Concat(words.Take(2).Select(w => char.ToUpperInvariant(w[0])));
        BackEllipse.Fill = new SolidColorBrush(Palette[Math.Abs(userId) % Palette.Length]);

        var image = ImageLoader.TryLoad(avatarPath);
        PhotoEllipse.Fill = image is null ? null : new ImageBrush(image) { Stretch = Stretch.UniformToFill };
    }
}
