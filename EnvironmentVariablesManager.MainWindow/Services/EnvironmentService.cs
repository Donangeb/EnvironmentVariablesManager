using EnvironmentVariablesManager.Models;
using Microsoft.Extensions.Logging;

namespace EnvironmentVariablesManager.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly ILogger<EnvironmentService> _logger;
        private readonly AppSettings _appSettings;

        public EnvironmentService(ILogger<EnvironmentService> logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public List<EnvironmentVariable> LoadEnvironmentVariables()
        {
            var variables = new List<EnvironmentVariable>();

            if (_appSettings?.EnvironmentVariables == null)
            {
                _logger.LogWarning("Настройки EnvironmentVariables не найдены в appsettings.json");
                return variables;
            }

            _logger.LogInformation("Загрузка переменных из appsettings: {Count} переменных", _appSettings.EnvironmentVariables.Count);

            foreach (var varName in _appSettings.EnvironmentVariables)
            {
                string value = Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.User)
                            ?? Environment.GetEnvironmentVariable(varName, EnvironmentVariableTarget.Process)
                            ?? GetDefaultValue(varName);

                variables.Add(new EnvironmentVariable
                {
                    Name = varName,
                    Value = value,
                    Comment = $"Системная переменная: {varName}"
                });

                _logger.LogInformation("Загружена переменная: {Name} = {Value}", varName, value);
            }

            return variables;
        }

        // Остальные методы без изменений...
        public void SaveEnvironmentVariables(List<EnvironmentVariable> variables)
        {
            foreach (var variable in variables)
            {
                try
                {
                    Environment.SetEnvironmentVariable(variable.Name, variable.Value, EnvironmentVariableTarget.User);
                    _logger.LogInformation("Сохранена переменная: {Name} = {Value}", variable.Name, variable.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при сохранении переменной {Name}", variable.Name);
                    throw;
                }
            }
        }

        public EnvironmentVariable GetEnvironmentVariable(string name)
        {
            string value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User)
                        ?? Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process)
                        ?? GetDefaultValue(name);

            return new EnvironmentVariable
            {
                Name = name,
                Value = value,
                Comment = $"Системная переменная: {name}"
            };
        }

        public void SetEnvironmentVariable(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
            _logger.LogInformation("Установлена переменная: {Name} = {Value}", name, value);
        }

        private string GetDefaultValue(string varName)
        {
            if (_appSettings?.DefaultValues != null && _appSettings.DefaultValues.TryGetValue(varName, out var defaultValue))
            {
                _logger.LogInformation("Использовано значение по умолчанию для переменной: {Name}", varName);
                return defaultValue;
            }

            return string.Empty;
        }
    }
}