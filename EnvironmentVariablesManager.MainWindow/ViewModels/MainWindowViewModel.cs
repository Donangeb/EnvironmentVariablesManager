using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EnvironmentVariablesManager.MainWindow;
using EnvironmentVariablesManager.Services;
using Microsoft.Extensions.Logging;
using EnvironmentVariablesManager.Models;

namespace EnvironmentVariablesManager.MainWindow.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IEnvironmentService _environmentService;
        private readonly ILogger<MainWindowViewModel> _logger;
        private ObservableCollection<EnvironmentVariable> _environmentVariables;

        public ObservableCollection<EnvironmentVariable> EnvironmentVariables
        {
            get => _environmentVariables;
            set
            {
                _environmentVariables = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(IEnvironmentService environmentService, ILogger<MainWindowViewModel> logger)
        {
            _environmentService = environmentService;
            _logger = logger;
            EnvironmentVariables = new ObservableCollection<EnvironmentVariable>();

            LoadEnvironmentVariables();
        }

        public void LoadEnvironmentVariables()
        {
            try
            {
                var variables = _environmentService.LoadEnvironmentVariables();
                EnvironmentVariables = new ObservableCollection<EnvironmentVariable>(variables);
                _logger.LogInformation("Переменные среды успешно загружены");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке переменных среды");
            }
        }

        public void SaveEnvironmentVariables()
        {
            try
            {
                _environmentService.SaveEnvironmentVariables(EnvironmentVariables.ToList());
                _logger.LogInformation("Переменные среды успешно сохранены");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении переменных среды");
                throw;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
