// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionEventing.Demo.Wpf.Services;
using ReflectionEventing.Demo.Wpf.ViewModels;

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
            c.SetBasePath(Directory.GetCurrentDirectory());
            c.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .ConfigureServices(
            (context, services) =>
            {
                services.AddHostedService<ApplicationHostService>();
                services.AddHostedService<BackgroundTickService>();

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddEventBus(e =>
                {
                    e.AddConsumer<MainWindowViewModel>();
                });
            }
        )
        .Build();

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        Host.Start();
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
