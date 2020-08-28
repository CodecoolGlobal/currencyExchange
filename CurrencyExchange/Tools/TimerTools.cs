using System.Timers;

namespace CurrencyExchange.Tools
{
    public class TimerTools
    {
        public static Timer GenerateTimer(int interval)
        {
            //interval is in minutes
            interval *= 60000;
            //1000 units = 1 second
            //10000 = 10 seconds
            //60000 = 1 minute
            //3600000 = 1 hour
            Timer timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = interval;
            timer.Enabled = true;
            timer.Start();
            return timer;
        }
    }
}
