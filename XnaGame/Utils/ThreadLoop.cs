using System;
using System.Diagnostics;
using System.Threading;

namespace XnaGame.Utils
{
    public class ThreadLoop
    {
        private readonly Action<float> action;
        private readonly Thread thread;
        private Stopwatch stopwatch;

        public ThreadLoop(Action<float> action)
        {
            this.action = action;
            thread = new Thread(Update);
        }

        public void Start()
        {
            thread.Start();
        }

        private void Update()
        {
            Thread.CurrentThread.IsBackground = true;
            stopwatch = Stopwatch.StartNew();

            while (true)
            {
                float elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000f;
                stopwatch.Restart();
                action(elapsedSeconds);
                Thread.Sleep(1);
            }
        }
    }
}
