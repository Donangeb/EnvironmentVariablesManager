using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVariablesManager.Models
{
    public class EnvironmentVariable
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}
