// See https://aka.ms/new-console-template for more information

namespace UpyunAction // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteLine($"Hello World!");

            WriteLine($"args: {string.Join(", ", args)}");

            var envKeys = Environment.GetEnvironmentVariables().Keys;
            foreach (var key in envKeys)
            {
                WriteLine($"Environment: {key}");
            }


            WriteLine($"Environment: INPUT_UPYUN_TOKEN: {Environment.GetEnvironmentVariable("INPUT_UPYUN_TOKEN")}");

            Console.WriteLine("::set-output name=upyun_response::strawberry");

        }

        public static void WriteLine(string message)
        {
            Console.WriteLine($"UpyunAction {DateTime.Now}: {message}");
        }

    }
}