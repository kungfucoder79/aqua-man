using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public class AquaPinController : IAquaPinController
    {
        #region Members
        //the gpio pins and their respective pin numbers on the board
        private GpioPin _fillPin;

        private GpioPin _wastePin;

        private GpioPin _inPin;

        private GpioPin _outPin;

        private GpioPin _pumpPin;

        private GpioPin _inPin1;

        private GpioPin _inPin2;

        private GpioPin _inPin3;

        private GpioPin _inPin4;

        private const GpioPinValue _0 = GpioPinValue.High;
        private const GpioPinValue _1 = GpioPinValue.Low;

        private GpioPinValue[][] _feederSequences = new GpioPinValue[][]{
            new[] {_0, _1, _1, _1},
            new[] {_1, _0, _1, _1},
            new[] {_1, _1, _0, _1},
            new[] {_1, _1, _1, _0}};


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
        public AquaPinController()
        {
            _fillDrainInterval = TimeSpan.FromSeconds(15);
            _timer_fill = new Timer(Timer_fill_Tick, null, Timeout.Infinite, Timeout.Infinite);

            _timer_drain = new Timer(Timer_drain_Tick, null, Timeout.Infinite, Timeout.Infinite);

            _timer_pumpOnDelay = new Timer(_timer_pumpOnDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
            _timer_pumpOffDelay = new Timer(_timer_pumpOffDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);

            GpioController gpio = GpioController.Instance;

            // Below are the separate pins being used from the raspberry pi and how they are setup
            // need to assign pins, values, and driver output

            _pumpPin = InitializePin(gpio.Pin17, _0, GpioPinDriveMode.Output);

            _fillPin = InitializePin(gpio.Pin20, _0, GpioPinDriveMode.Output);

            _wastePin = InitializePin(gpio.Pin21, _0, GpioPinDriveMode.Output);

            _inPin = InitializePin(gpio.Pin19, _0, GpioPinDriveMode.Output);

            _outPin = InitializePin(gpio.Pin26, _0, GpioPinDriveMode.Output);

            _inPin1 = InitializePin(gpio.Pin23, _0, GpioPinDriveMode.Output);

            _inPin2 = InitializePin(gpio.Pin24, _0, GpioPinDriveMode.Output);

            _inPin3 = InitializePin(gpio.Pin25, _0, GpioPinDriveMode.Output);

            _inPin4 = InitializePin(gpio.Pin08, _0, GpioPinDriveMode.Output);
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

        private GpioPin InitializePin(GpioPin pin, GpioPinValue gpioPinValue, GpioPinDriveMode gpioPinDriveMode)
        {
            pin.PinMode = gpioPinDriveMode;
            pin.Write(gpioPinValue);
            return pin;
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
            _inPin.Write(_0);

            _fillPin.Write(_0);

            _outPin.Write(_0);

            _wastePin.Write(_0);
        }

        /// <summary>
        /// Timer function for turning the pump on.  When the tick interval is reached, this function will run and set the 
        /// <see cref="_pumpPin"/> low (which turns it on)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_pumpOnDelay_Tick(object sender)
        {
            _pumpPin.Write(_1);
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
            _pumpPin.Write(_0);
            _timer_pumpOffDelay.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// GUI click event for filling.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valve.
        /// </summary>
        /// <returns>The value of the <see cref="_pumppinValue"/></returns>
        public void Fill()
        {
            _inPin.Write(_1);

            _fillPin.Write(_1);

            _outPin.Write(_0);

            _wastePin.Write(_0);

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
            _outPin.Write(_1);

            _wastePin.Write(_1);

            _inPin.Write(_0);

            _fillPin.Write(_0);

            _timer_drain.Change(_fillDrainInterval, Timeout.InfiniteTimeSpan);
            _timer_pumpOnDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Starts the feeding auger sequence with the specified number of times to step.
        /// </summary>
        /// <param name="numberOfTimes"></param>
        public async void FeedMe(int numberOfTimes)
        {
            int delay = 1;
            for (int i = 0; i < numberOfTimes; i++)
            {
                for (int j = 0; j < _feederSequences[0].Length; j++)
                {
                    _inPin1.Write(_feederSequences[j][0]);
                    _inPin2.Write(_feederSequences[j][1]);
                    _inPin3.Write(_feederSequences[j][2]);
                    _inPin4.Write(_feederSequences[j][3]);
                    await Task.Delay(delay);
                }
            }
        }
        #endregion
    }
}
