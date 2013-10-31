using System;
using System.Runtime.InteropServices;

namespace BulletHell.Time
{
    class Timer
    {
        [DllImport("kernel32.dll")]
        private static extern long GetTickCount();

        private long StartTick = 0;
        public long Time
        {
            get
            {
                long currentTick = GetTickCount();
                return currentTick - StartTick;
            }
            set
            {
                StartTick = GetTickCount() - value;
            }
        }
        public Timer()
        {
            Reset();
        }
        public void Reset()
        {
            StartTick = GetTickCount();
        }
    }
}
