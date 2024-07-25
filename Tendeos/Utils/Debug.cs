using Microsoft.Xna.Framework;
using System;
using System.Threading;

namespace Tendeos.Utils
{
    public static class Debug
    {
        private static event Action showErrors;

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
                from = $"{from}.{exception.GetType().Name}";
                ErrorBox(from, current);
                current = current.InnerException;
            }
        }

        private static void ErrorBox(string from, Exception exception)
        {
            showErrors += () => MessageBox.Show(from,
                $"Handled {exception.GetType().Name} \"{exception.Message}\"\n{exception.StackTrace}",
                MessageBox.Type.Error);
        }

        public static void ShowErrors()
        {
            showErrors?.Invoke();
        }
    }
}