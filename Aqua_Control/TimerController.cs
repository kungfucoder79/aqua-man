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
        public event EventHandler DrainStart;
        public event EventHandler FillDone;
        public event EventHandler FillStart;
        #endregion

        #region ctor
        public TimerController()
        {
            //_fillDrainInterval = TimeSpan.FromSeconds(15);
            //_timer_fill = new Timer(Timer_fill_Tick, null, Timeout.Infinite, Timeout.Infinite);

            //_timer_drain = new Timer(Timer_drain_Tick, null, Timeout.Infinite, Timeout.Infinite);

            //_timer_pumpOnDelay = new Timer(_timer_pumpOnDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
            //_timer_pumpOffDelay = new Timer(_timer_pumpOffDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);

        }
        #endregion

        #region Methods

        protected virtual void OnDrainDone(EventArgs e)
        {
            DrainDone?.Invoke(this, e);
        }

        protected virtual void OnDrainStart(EventArgs e)
        {
            DrainStart?.Invoke(this, e);
        }
        protected virtual void OnFillDone(EventArgs e)
        {
            FillDone?.Invoke(this, e);
        }

        protected virtual void OnFillStart(EventArgs e)
        {
            FillStart?.Invoke(this, e);
        }

        #endregion
    }
}
