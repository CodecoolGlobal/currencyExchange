using System.Timers;

namespace CurrencyExchange.Tools
{
    public class TimerTool
    {
        public static Timer GenerateTimer(int interval)
        {
            Timer timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = interval;
            timer.Enabled = true;
            timer.Start();
            return timer;
        }
    }
}
