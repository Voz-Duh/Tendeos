using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace XnaGame.Utils
{
    public static class Debug
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
        [DllImport("user32.dll")]
        public static extern int MessageBox(nint hwnd, string text, string caption, uint type);

        private static int done;
        public static bool Done => done == 0;

        public static void Create()
        {
            AllocConsole();

            TextReader reader = new StreamReader(Console.OpenStandardInput());
            Console.SetIn(reader);
        }

        public static void Destroy()
        {
            FreeConsole();
        }

        public static void Safe(Game game, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Error(ex);
                game.Exit();
            }
        }

        public static void Error(Exception exception)
        {
            string from = Core.ApplicationName;
            Exception current = exception;
            while (current != null)
            {
                ErrorBox(from, current);
                from = $"{from}.{exception.GetType().Name}";
                current = current.InnerException;
            }
        }

        private static void ErrorBox(string from, Exception exception)
        {
            done++;
            Task.Run(() =>
            {
                MessageBox(0, $"Handled {exception.GetType().Name} \"{exception.Message}\"\n{exception.StackTrace}", from, 0);
                done--;
            });
        }

        public static void WaitForDone()
        {
            while (!Done) { }
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Color(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public static void LogColored(params (string, ConsoleColor)[] message)
        {
            foreach (var (text, color) in message)
            {
                Console.ForegroundColor = color;
                Console.Write(text);
            }
            Console.Write('\n');
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
