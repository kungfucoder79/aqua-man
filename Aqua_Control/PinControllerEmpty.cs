using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aqua_Control
{
    /// <summary>
    /// Empty impelmentation for testing on a device with out pins
    /// </summary>
    public class PinControllerEmpty : IAquaPinController
    {
        #region Members
        // Dispatcher timer setup and classes.  The interval time seen below is abritary and was used for 
        // testing.  These values will change based on what is best for the project in opening the water valves
        private Timer _timer_fill;
        private Timer _timer_drain;
        private Timer _timer_pumpOnDelay;
        private Timer _timer_pumpOffDelay;
        private TimeSpan _fillDrainInterval;
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new <see cref="AquaGPIO"/> object by initialzing the gpio pins for the raspberry pi
        /// </summary>
        public PinControllerEmpty()
        {
            _fillDrainInterval = TimeSpan.FromSeconds(15);
            _timer_fill = new Timer(Timer_fill_Tick, null, Timeout.Infinite, Timeout.Infinite);

            _timer_drain = new Timer(Timer_drain_Tick, null, Timeout.Infinite, Timeout.Infinite);

            _timer_pumpOnDelay = new Timer(_timer_pumpOnDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
            _timer_pumpOffDelay = new Timer(_timer_pumpOffDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
        }

        #endregion

        #region Properties
        public event EventHandler DrainDone;
        public event EventHandler FillDone;
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

        /// <summary>
        /// Timer function for Filling.  When the tick interval is reached, this function will run and set the 
        /// GPIO high (which closed the valves) and stops the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_fill_Tick(object sender)
        {
            TurnValvesOff();

            _timer_fill.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            _timer_pumpOffDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);

            OnFillDone(EventArgs.Empty);
        }

        /// <summary>
        /// Timer function for drain.  When the tick interval is reached, this function will run and set the 
        /// GPIO high (which closed the valves) and stops the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_drain_Tick(object sender)
        {
            TurnValvesOff();

            _timer_drain.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            _timer_pumpOffDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);

            OnDrainDone(EventArgs.Empty);
        }

        /// <summary>
        /// Set the pins to high which turns the valves off
        /// </summary>
        private void TurnValvesOff()
        {
            Console.WriteLine($"----------------{DateTime.Now}: TURNING VALVES OFF");
        }

        /// <summary>
        /// Timer function for turning the pump on.  When the tick interval is reached, this function will run and set the 
        /// <see cref="_pumpPin"/> low (which turns it on)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_pumpOnDelay_Tick(object sender)
        {
            Console.WriteLine($"----------------{DateTime.Now}: TURNING PUMP ON");
            _timer_pumpOnDelay.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Timer function for turning the pump on.  When the tick interval is reached, this function will run and set the 
        /// <see cref="_pumpPin"/> low (which turns it off)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_pumpOffDelay_Tick(object sender)
        {
            Console.WriteLine($"----------------{DateTime.Now}: TURNING PUMP OFF");
            _timer_pumpOffDelay.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// GUI click event for filling.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valve.
        /// </summary>
        /// <returns>The value of the <see cref="_pumppinValue"/></returns>
        public void Fill()
        {
            Console.WriteLine($"----------------{DateTime.Now}: FILLING ON");

            _timer_fill.Change(_fillDrainInterval, Timeout.InfiniteTimeSpan);
            _timer_pumpOnDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// GUI click event for draining.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valves.
        /// </summary>
        /// <returns>The value of the <see cref="_pumppinValue"/></returns>
        public void Drain()
        {
            Console.WriteLine($"----------------{DateTime.Now}: TURNING DRAIN ON");

            _timer_drain.Change(_fillDrainInterval, Timeout.InfiniteTimeSpan);
            _timer_pumpOnDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Starts the feeding auger sequence with the specified number of times to step.
        /// </summary>
        /// <param name="numberOfTimes"></param>
        public async void FeedMe(int numberOfTimes)
        {
            Console.WriteLine($"----------------{DateTime.Now}: FEEDING");
            await Task.Delay(1);
        }
        #endregion
    }
}
