using AIProcessDiagramDrafting.ViewModels;

namespace AIProcessDiagramDrafting;

public partial class MainPage : ContentPage
{
    public MainPage(ProcessDiagramViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
