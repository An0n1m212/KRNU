using System.Windows;
using System.Windows.Controls;

namespace TestingSystem;

/// <summary>v0.1: контракт (моделі та інтерфейси). Вікно-заглушка показує, що проєкт збирається і запускається.</summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var asm = typeof(TestingSystem.Models.User).Assembly;
        int models = asm.GetTypes().Count(t => t.Namespace == "TestingSystem.Models");
        int contracts = asm.GetTypes().Count(t => t.Namespace == "TestingSystem.Abstractions");

        var panel = new StackPanel { Margin = new Thickness(24) };
        panel.Children.Add(new TextBlock { Text = "Testing System v0.1 — контракт", FontSize = 22, FontWeight = FontWeights.Bold });
        panel.Children.Add(new TextBlock { Text = $"Типів у Models: {models}; у Abstractions: {contracts}", Margin = new Thickness(0, 12, 0, 0) });
        panel.Children.Add(new TextBlock { Text = "Далі: v0.2 — архітектура, вхід, ролі.", Margin = new Thickness(0, 8, 0, 0) });

        new Window
        {
            Title = "Testing System v0.1", Width = 520, Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterScreen, Content = panel
        }.Show();
    }
}
