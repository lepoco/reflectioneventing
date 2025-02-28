// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.Demo.Wpf.Services;
using ReflectionEventing.Demo.Wpf.ViewModels;
using ReflectionEventing.DependencyInjection;
using ReflectionEventing.DependencyInjection.Services;

namespace ReflectionEventing.Demo.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly IHost Host = Microsoft
        .Extensions.Hosting.Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(c =>
        {
            _ = c.SetBasePath(Directory.GetCurrentDirectory());
            _ = c.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .ConfigureServices(
            (context, services) =>
            {
                _ = services.AddHostedService<ApplicationHostService>();
                _ = services.AddHostedService<BackgroundTickService>();

                _ = services.AddSingleton<MainWindow>();
                _ = services.AddSingleton<MainWindowViewModel>();

                _ = services.AddEventBus(e =>
                {
                    e.Options.UseEventPolymorphism = true;
                    e.Options.UseEventsQueue = true;
                    e.Options.QueueMode = ProcessingMode.Parallel;

                    e.UseBackgroundService<DependencyInjectionQueueProcessor>();
                    e.AddAllConsumers(Assembly.GetExecutingAssembly());
                });
            }
        )
        .Build();

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await Host.StartAsync();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await Host.StopAsync();

        Host.Dispose();
    }
}
