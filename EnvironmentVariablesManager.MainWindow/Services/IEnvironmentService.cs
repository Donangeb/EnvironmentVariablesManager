using EnvironmentVariablesManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EnvironmentVariablesManager.Services
{
    public interface IEnvironmentService
    {
        List<EnvironmentVariable> LoadEnvironmentVariables();
        void SaveEnvironmentVariables(List<EnvironmentVariable> variables);
        EnvironmentVariable GetEnvironmentVariable(string name);
        void SetEnvironmentVariable(string name, string value);
    }
}