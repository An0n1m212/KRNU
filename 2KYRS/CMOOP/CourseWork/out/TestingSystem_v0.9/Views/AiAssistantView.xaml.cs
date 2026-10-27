using System.Windows.Controls;
using TestingSystem.ViewModels;

namespace TestingSystem.Views;

public partial class AiAssistantView : UserControl
{
    public AiAssistantView(AiAssistantViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += (_, _) => viewModel.OnShown();
        Unloaded += (_, _) => viewModel.OnHidden();
    }
}
