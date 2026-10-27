using System.Windows;
using System.Windows.Controls;

namespace TestingSystem.Views;

/// <summary>Результаты ТОЛЬКО текущего ученика: разбор ошибок и скачивание сертификатов.</summary>
public partial class MyResultsView : UserControl
{
    private readonly AppServices _s;

    public MyResultsView(AppServices services)
    {
        InitializeComponent();
        _s = services;

        Loaded += (_, _) =>
        {
            _s.Localization.LanguageChanged -= OnLanguageChanged;
            _s.Localization.LanguageChanged += OnLanguageChanged;
            Reload();
        };
        Unloaded += (_, _) => _s.Localization.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) => Reload();

    private void Reload()
    {
        var me = _s.Auth.GetCurrentUser();
        if (me is null) return;
        ResultsGrid.ItemsSource = new ResultRowFactory(_s).Build(_s.Results.GetByStudent(me.Id));
    }

    private void Details_Click(object sender, RoutedEventArgs e) => ShowDetails();
    private void ResultsGrid_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => ShowDetails();

    private void ShowDetails()
    {
        if (ResultsGrid.SelectedItem is not ResultRow row)
        {
            MessageBox.Show(_s.Localization["Msg_SelectResult"]); return;
        }

        // Правильные ответы и пояснения — только если учитель это разрешил
        var test = _s.Tests.GetById(row.Source.TestId);
        bool showCorrect = test?.AllowShowExplanations ?? false;

        new ResultDetailsWindow(_s, row.Source, row.Test, showCorrect) { Owner = Window.GetWindow(this) }.ShowDialog();
    }

    private void Certificate_Click(object sender, RoutedEventArgs e)
    {
        if (ResultsGrid.SelectedItem is not ResultRow row)
        {
            MessageBox.Show(_s.Localization["Msg_SelectResult"]); return;
        }
        CertificateHelper.Download(_s, row.Source, Window.GetWindow(this));
    }
}
