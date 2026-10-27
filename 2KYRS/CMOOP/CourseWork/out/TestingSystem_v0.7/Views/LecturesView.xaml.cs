using System.Windows.Controls;
using TestingSystem.ViewModels;

namespace TestingSystem.Views;

/// <summary>Тонкий View: уся логіка у LecturesViewModel; code-behind лише передає життєвий цикл.</summary>
public partial class LecturesView : UserControl
{
    public LecturesView(LecturesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += (_, _) => viewModel.OnShown();
        Unloaded += (_, _) => viewModel.OnHidden();
    }
}
