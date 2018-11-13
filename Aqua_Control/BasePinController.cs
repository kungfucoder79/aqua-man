using System;
using System.Collections.Generic;
using System.Text;

namespace Aqua_Control
{
    /// <summary>
    /// base class for the common pin controller logic
    /// </summary>
    public abstract class BasePinController
    {
        #region Properties
        protected bool _V1Latch = false;
        protected bool _V2Latch = false;
        protected bool _V3Latch = false;
        protected bool _V4Latch = false;
        protected bool _V5Latch = false;

        protected TimerController TimerController { get; private set; }
        
        /// <summary>
        /// Gets if the <see cref="BasePinController"/> is currently running a fill sequence
        /// </summary>
        public bool IsFillActive { get; protected set; }

        /// <summary>
        /// Gets if the <see cref="BasePinController"/> is currently running a drain sequence
        /// </summary>
        public bool IsDrainActive { get; protected set; }
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new <see cref="BasePinController"/> object by initialzing the timer controller
        /// </summary>
        public BasePinController(IEnumerable<DateTime?> feedingTimes)
        {
            //Console.WriteLine($"--------------{DateTime.Now}: Initializing Pin Controller");
            TimerController = new TimerController(feedingTimes);
            TimerController.PumpOff += _timerController_PumpOff;
            TimerController.PumpOn += _timerController_PumpOn;
            TimerController.FeederStart += _timerController_FeederStart;
            IsFillActive = false;
            IsDrainActive = false;
        }
        #endregion

        #region Methods
        protected abstract void _timerController_FeederStart(object sender, EventArgs e);
        
        protected abstract void _timerController_PumpOn(object sender, EventArgs e);

        protected abstract void _timerController_PumpOff(object sender, EventArgs e);

        protected abstract void TurnValvesOff();

        public void UpdateFeedingTimes(IEnumerable<DateTime?> _feedingTimes)
        {
            TimerController.FeedingTimes.Clear();
            foreach(DateTime? feedingTime in _feedingTimes)
            {
                TimerController.FeedingTimes.Add(feedingTime);
            }
        }

        public void Stop()
        {
            TurnValvesOff();
            TimerController.SetPumpOffDelay();
        }
        #endregion
    }
}
