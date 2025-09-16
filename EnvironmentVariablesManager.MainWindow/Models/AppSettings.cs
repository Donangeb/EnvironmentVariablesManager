using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVariablesManager.Models
{
        public class AppSettings
        {
            public List<string> EnvironmentVariables { get; set; } = new List<string>();
            public Dictionary<string, string> DefaultValues { get; set; } = new Dictionary<string, string>();
        }
}
