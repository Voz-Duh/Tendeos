using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Tendeos.Utils
{
    public static class Debug
    {
        private static int done;
        public static bool Done => done == 0;

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
                MessageBox.Show(from, $"Handled {exception.GetType().Name} \"{exception.Message}\"\n{exception.StackTrace}", MessageBox.Type.Error);
                done--;
            });
        }
        public static void WaitForDone()
        {
            while (!Done) { }
        }
    }
}
