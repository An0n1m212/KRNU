using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Services.Ai;

namespace TestingSystem.Views;

/// <summary>Тиждень 8 (ТЗ п.3.2.4, п.3.7.3): DPAPI-сховище, вбудована інструкція, «Перевірити з'єднання».</summary>
public partial class AiSettingsWindow : Window
{
    private readonly ServiceProvider _sp;

    public AiSettingsWindow(ServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
        ProviderBox.ItemsSource = Enum.GetValues<AiProviderKind>();
        var cur = _sp.GetRequiredService<IAiSettingsStore>().Current;
        ProviderBox.SelectedItem = cur.Provider;
        UrlBox.Text = cur.BaseUrl;
        ModelBox.Text = cur.Model;
        TimeoutBox.Text = cur.TimeoutSeconds.ToString();
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        int timeout = int.TryParse(TimeoutBox.Text, out var t) ? Math.Clamp(t, 5, 300) : 90;
        _sp.GetRequiredService<IAiSettingsStore>().Save(new AiSettings(
            ProviderBox.SelectedItem is AiProviderKind k ? k : AiProviderKind.Mock,
            UrlBox.Text.Trim(), ModelBox.Text.Trim(), KeyBox.Password, timeout));
        StatusText.Text = "Збережено.";
    }

    private async void OnTest(object sender, RoutedEventArgs e)
    {
        try
        {
            int timeout = int.TryParse(TimeoutBox.Text, out var t) ? Math.Clamp(t, 5, 300) : 90;
            var settings = new AiSettings(
                ProviderBox.SelectedItem is AiProviderKind k ? k : AiProviderKind.Mock,
                UrlBox.Text.Trim(), ModelBox.Text.Trim(),
                KeyBox.Password.Length > 0 ? KeyBox.Password : _sp.GetRequiredService<IAiSettingsStore>().Current.ApiKey,
                timeout);
            var client = _sp.GetRequiredService<IAiClient>();
            string answer = await client.CompleteAsync(settings,
                new AiRequest(AiTask.Ping, "ping", "ping"), CancellationToken.None);
            StatusText.Text = "Відповідь: " + answer;
        }
        catch (Exception ex)
        {
            StatusText.Text = AiErrorFormatter.Describe(_sp.GetRequiredService<ILocalizationService>(), ex);
        }
    }
}
