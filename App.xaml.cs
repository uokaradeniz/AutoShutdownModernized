using System.Windows;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoShutdownModernized.Localization;
using AutoShutdownModernized.Services;
using AutoShutdownModernized.ViewModels;

namespace AutoShutdownModernized
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IHost? _host;

        public static IServiceProvider? ServiceProvider { get; private set; }

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<IShutdownService, ShutdownService>();
                    services.AddSingleton<ShutdownTrackerService>();
                    services.AddSingleton<IShutdownTrackerService>(provider => provider.GetRequiredService<ShutdownTrackerService>());
                    services.AddHostedService(provider => provider.GetRequiredService<ShutdownTrackerService>());
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();

            ServiceProvider = _host.Services;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LocalizationManager.ApplySupportedCulture(CultureInfo.CurrentUICulture);
            await _host!.StartAsync();
            
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                try
                {
                    var shutdownService = _host.Services.GetService<IShutdownService>();
                    shutdownService?.CancelShutdown();
                }
                finally
                {
                    await _host.StopAsync();
                    _host.Dispose();
                }
            }
            base.OnExit(e);
        }
    }
}