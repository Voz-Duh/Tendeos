using System;
using System.Diagnostics;
using System.Threading;

namespace Tendeos.Utils
{
    public class ThreadLoop
    {
        private static Action<bool> setPause;

        private readonly Action<float> action;
        private readonly Thread thread;
        private readonly int tickFrom, tickTo;
        private Stopwatch stopwatch;
        private bool abort;
        public bool paused;

        public ThreadLoop(Action<float> action, int tick = 1) : this(action, tick, tick)
        {
        }

        public ThreadLoop(Action<float> action, int tickFrom, int tickTo)
        {
            this.action = action;
            this.tickFrom = tickFrom;
            this.tickTo = tickTo;
            thread = new Thread(Update);
            abort = false;
            paused = false;
            setPause += Pause;
        }

        ~ThreadLoop()
        {
            setPause -= Pause;
            Abort();
        }

        public void Abort() => abort = true;

        public static void SetPause(bool paused) => setPause(paused);

        private void Pause(bool paused) => this.paused = paused;

        public void Start()
        {
            thread.Start();
        }

        private void Update()
        {
            Thread.CurrentThread.IsBackground = true;
            stopwatch = Stopwatch.StartNew();

            while (!abort)
            {
                float elapsedSeconds = paused ? 0 : stopwatch.ElapsedMilliseconds / 1000f;
                stopwatch.Restart();
                if (!paused) action(elapsedSeconds);
                Thread.Sleep(URandom.SInt(tickFrom, tickTo));
            }
        }
    }
}