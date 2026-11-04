using Microsoft.Extensions.Logging;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;
using AIProcessDiagramDrafting.Services;
using AIProcessDiagramDrafting.ViewModels;

namespace AIProcessDiagramDrafting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JBaF5cXmRCf1NpRmNGfV5yckVHYlZVRXxdQk0DNHVRdkdlWXZdcHVXQmFeUEZyX0RWYEw=");

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IWorkflowExtractionService, AzureOpenAIWorkflowExtractionService>();
        builder.Services.AddSingleton<WorkflowResponseValidator>();
        builder.Services.AddSingleton<DiagramDraftBuilder>();
        builder.Services.AddSingleton<ProcessDiagramViewModel>();
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
