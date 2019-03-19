using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Aqua_Control
{
    public class TimerController
    {
        #region Memebers
        private Timer _timer_feeder;
        private Timer _timer_pumpOnDelay;
        private Timer _timer_pumpOffDelay;
        private TimeSpan _feedingInterval;
        #endregion

        #region Properties
        public event EventHandler PumpOn;
        public event EventHandler PumpOff;
        public event EventHandler FeederStart;
        public List<DateTime?> FeedingTimes { get; private set; }
        #endregion

        #region ctor
        public TimerController(IEnumerable<DateTime?> feedingTimes)
        {
            FeedingTimes = feedingTimes.ToList();

            _feedingInterval = TimeSpan.FromSeconds(2);

            _timer_feeder = new Timer(CheckFeedingTimes, null, TimeSpan.Zero, _feedingInterval);

            _timer_pumpOnDelay = new Timer(_timer_pumpOnDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
            _timer_pumpOffDelay = new Timer(_timer_pumpOffDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region Methods
        protected virtual void OnPumpOn(EventArgs e)
        {
            PumpOn?.Invoke(this, e);
        }

        protected virtual void OnPumpOff(EventArgs e)
        {
            PumpOff?.Invoke(this, e);
        }

        protected virtual void OnFeederStart(EventArgs e)
        {
            FeederStart?.Invoke(this, e);
        }

        public void SetPumpOffDelay()
        {
            _timer_pumpOffDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        }

        private void _timer_pumpOnDelay_Tick(object sender)
        {
            _timer_pumpOnDelay.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            OnPumpOn(EventArgs.Empty);
        }

        private void _timer_pumpOffDelay_Tick(object sender)
        {
            _timer_pumpOffDelay.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            OnPumpOff(EventArgs.Empty);
        }

        public void SetPumpOnDelay()
        {
            _timer_pumpOnDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        }

        private void CheckFeedingTimes(object state)
        {
            foreach (DateTime feedingTime in FeedingTimes.Where(p => p != null).ToArray())
            {
                if (IsTimeReadyToRun(feedingTime, _feedingInterval))
                {
                    OnFeederStart(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets whether the time to check lands with in the interval/>
        /// </summary>
        /// <param name="timeToCheck"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static bool IsTimeReadyToRun(DateTime timeToCheck, TimeSpan interval)
        {
            DateTime currentTime = GetCurrentDayTime(DateTime.Now);
            DateTime currentTimeToCheck = GetCurrentDayTime(timeToCheck);
            DateTime lastTime = currentTime - interval;
            return (lastTime <= currentTimeToCheck && currentTimeToCheck < currentTime);
        }

        /// <summary>
        /// Gets the Date time with the current year month and day
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetCurrentDayTime(DateTime dt)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, dt.Hour, dt.Minute, dt.Second);
        }
        #endregion
    }
}
