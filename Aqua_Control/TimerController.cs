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
                if (IsFeederReadyToRun(feedingTime))
                {
                    OnFeederStart(EventArgs.Empty);
                }
            }
        }
        private bool IsFeederReadyToRun(DateTime timeToCheck)
        {
            DateTime currentTime = GetCurrentDayTime(DateTime.Now);
            DateTime currentTimeToCheck = GetCurrentDayTime(timeToCheck);
            DateTime lastTime = currentTime - _feedingInterval;
            return (lastTime <= currentTimeToCheck && currentTimeToCheck < currentTime);
        }

        private DateTime GetCurrentDayTime(DateTime dt)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, dt.Hour, dt.Minute, dt.Second);
        }
        #endregion
    }
}
