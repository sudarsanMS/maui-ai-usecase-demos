using Microsoft.Extensions.DependencyInjection;

namespace AIProcessDiagramDrafting;

public partial class App : Application
{
    private readonly IServiceProvider serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(serviceProvider.GetRequiredService<MainPage>());
}
