using EnvironmentVariablesManager.Logging;
using EnvironmentVariablesManager.MainWindow.ViewModels;
using EnvironmentVariablesManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EnvironmentVariablesManager.MainWindow
{
    public partial class MainWindow : Window
    {
        private readonly IHost _host;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _host = CreateHostBuilder().Build();
            _viewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
            DataContext = _viewModel;

            // Загружаем данные после полной инициализации окна
            Loaded += MainWindow_Loaded;

            // Обработка перемещения окна
            MouseDown += Window_MouseDown;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Используем Dispatcher для загрузки данных после рендеринга
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                _viewModel.LoadEnvironmentVariables();
            }));
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Конфигурация
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    var appSettings = new Models.AppSettings();
                    configuration.Bind(appSettings);
                    services.AddSingleton(appSettings);

                    // Логирование
                    var logFileName = $"test-sms-wpf-app-{DateTime.Now:yyyyMMdd}.log";
                    var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", logFileName);

                    services.AddLogging(builder =>
                    {
                        builder.ClearProviders();
                        builder.AddProvider(new FileLoggerProvider(logFilePath));
                        builder.SetMinimumLevel(LogLevel.Information);
                    });

                    // Сервисы
                    services.AddSingleton<IEnvironmentService, Services.EnvironmentService>();
                    services.AddSingleton<MainWindowViewModel>();
                });
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.SaveEnvironmentVariables();
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении переменных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _viewModel.SaveEnvironmentVariables();
                _host.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении переменных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            base.OnClosing(e);
        }
    }
}