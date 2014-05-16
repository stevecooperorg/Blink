using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink.Util
{
    public static class Logging
    {
        private static Stopwatch stopwatch;
        private static long previousLog = 0;

        public static void Log(string message)
        {
            if (stopwatch == null)
            {
                stopwatch = Stopwatch.StartNew();
            }

            var now = stopwatch.ElapsedMilliseconds;
            var msSinceLastLog = now - previousLog;
            previousLog = now;

            Debug.WriteLine(string.Format("{0:D10}ms: {1}", msSinceLastLog , message));
        }

    }
}
