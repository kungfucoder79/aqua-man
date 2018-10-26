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
        private Timer _timer_fill;
        private Timer _timer_drain;
        private Timer _timer_feeder;
        private Timer _timer_pumpOnDelay;
        private Timer _timer_pumpOffDelay;
        private TimeSpan _fillDrainInterval;
        private TimeSpan _feedingInterval;
        /*= new DateTime?[] { new DateTime(2018, 10, 25, 23, 53, 00),
                                                              new DateTime(2018, 10, 25, 23, 53, 10),
                                                              new DateTime(2018, 10, 25, 23, 53, 13),
                                                            };*/
        #endregion

        #region Properties
        public event EventHandler DrainDone;
        public event EventHandler FillDone;
        public event EventHandler PumpOn;
        public event EventHandler PumpOff;
        public event EventHandler FeederStart;
        public List<DateTime?> FeedingTimes { get; private set; }
        #endregion

        #region ctor
        public TimerController()
        {
            FeedingTimes = new List<DateTime?>(5);

            _fillDrainInterval = TimeSpan.FromSeconds(2);
            _feedingInterval = TimeSpan.FromSeconds(2);

            _timer_fill = new Timer(Timer_fill_Tick, null, Timeout.Infinite, Timeout.Infinite);

            _timer_drain = new Timer(Timer_drain_Tick, null, Timeout.Infinite, Timeout.Infinite);

            _timer_feeder = new Timer(CheckFeedingTimes, null, TimeSpan.Zero, _feedingInterval);

            _timer_pumpOnDelay = new Timer(_timer_pumpOnDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
            _timer_pumpOffDelay = new Timer(_timer_pumpOffDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region Methods

        protected virtual void OnDrainDone(EventArgs e)
        {
            DrainDone?.Invoke(this, e);
        }

        protected virtual void OnFillDone(EventArgs e)
        {
            FillDone?.Invoke(this, e);
        }

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

        private void Timer_fill_Tick(object sender)
        {
            _timer_fill.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            _timer_pumpOffDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);

            OnFillDone(EventArgs.Empty);
        }

        private void Timer_drain_Tick(object sender)
        {
            _timer_drain.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            _timer_pumpOffDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);

            OnDrainDone(EventArgs.Empty);
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

        public void SetFillTimer()
        {
            _timer_fill.Change(_fillDrainInterval, Timeout.InfiniteTimeSpan);
            _timer_pumpOnDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        }

        public void SetDrainTimer()
        {
            _timer_drain.Change(_fillDrainInterval, Timeout.InfiniteTimeSpan);
            _timer_pumpOnDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        }

        private void CheckFeedingTimes(object state)
        {
            foreach (DateTime feedingTime in FeedingTimes.Where(p => p != null).ToArray())
            {
                if (IsFeederReadyToRun(feedingTime))
                    OnFeederStart(EventArgs.Empty);
            }
        }
        private bool IsFeederReadyToRun(DateTime timeToCheck)
        {
            DateTime currentTime = GetCurrentDayTime(DateTime.Now);
            DateTime lastTime = currentTime - _feedingInterval;
            return (lastTime <= timeToCheck && timeToCheck < currentTime);
        }

        private DateTime GetCurrentDayTime(DateTime dt)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, dt.Hour, dt.Minute, dt.Second);
        }
        #endregion
    }
}
