using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpyunAction.Utils
{
    public class GitHubActionsUtil
    {
        public static string GetEnv(string name)
        {
            if (!name.StartsWith("INPUT_"))
            {
                name = $"INPUT_{name}";
            }
            name = name.ToUpper();

            return Environment.GetEnvironmentVariable(name);
        }

        public static void SetOutput(string name, string value)
        {
            Console.WriteLine($"::set-output name={name}::{value}");
        }
    }
}
