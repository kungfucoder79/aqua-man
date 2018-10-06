using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aqua_Control
{
    public class TimerController
    {
        #region Memebers
        private Timer _timer_fill;
        private Timer _timer_drain;
        private Timer _timer_pumpOnDelay;
        private Timer _timer_pumpOffDelay;
        private TimeSpan _fillDrainInterval;
        #endregion

        #region Properties
        public event EventHandler DrainDone;
        public event EventHandler FillDone;
        public event EventHandler PumpOn;
        public event EventHandler PumpOff;

        #endregion

        #region ctor
        public TimerController()
        {
            _fillDrainInterval = TimeSpan.FromSeconds(15);
            _timer_fill = new Timer(Timer_fill_Tick, null, Timeout.Infinite, Timeout.Infinite);

            _timer_drain = new Timer(Timer_drain_Tick, null, Timeout.Infinite, Timeout.Infinite);

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
        #endregion
    }
}
