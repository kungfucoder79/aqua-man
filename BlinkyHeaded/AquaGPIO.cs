using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace Aqua_ControlUWP
{
    public class AquaGPIO
    {
        #region Members
        //the gpio pins and their respective pin numbers on the board
        private const int _fillPinID = 20;
        private GpioPin _fillPin;

        private const int _wastePinID = 21;
        private GpioPin _wastePin;

        private const int _inPinID = 19;
        private GpioPin _inPin;

        private const int _outPinID = 26;
        private GpioPin _outPin;

        private const int _pumpPinID = 16;
        private GpioPin _pumpPin;
        private GpioPinValue _pumppinValue;

        // Dispatcher timer setup and classes.  The interval time seen below is abritary and was used for 
        // testing.  These values will change based on what is best for the project in opening the water valves
        private DispatcherTimer _timer_fill;
        private DispatcherTimer _timer_drain;
        private Timer _timer_pumpOnDelay;
        private Timer _timer_pumpOffDelay;
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new <see cref="AquaGPIO"/> object by initialzing the gpio pins for the raspberry pi
        /// </summary>
        public AquaGPIO()
        {
            
            _timer_fill = new DispatcherTimer();
            _timer_fill.Interval = TimeSpan.FromSeconds(5);
            _timer_fill.Tick += Timer_fill_Tick;

            _timer_drain = new DispatcherTimer();
            _timer_drain.Interval = TimeSpan.FromSeconds(5);
            _timer_drain.Tick += Timer_drain_Tick;

            _timer_pumpOnDelay = new Timer(_timer_pumpOnDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);
            _timer_pumpOffDelay = new Timer(_timer_pumpOffDelay_Tick, null, Timeout.Infinite, Timeout.Infinite);

            GpioController gpio = GpioController.GetDefault();

            // Below are the separate pins being used from the raspberry pi and how they are setup
            // need to assign pins, values, and driver output
            _pumppinValue = GpioPinValue.High;
            InitializePin(gpio, out _pumpPin, _pumpPinID);

            InitializePin(gpio, out _fillPin, _fillPinID);

            InitializePin(gpio, out _wastePin, _wastePinID);

            InitializePin(gpio, out _inPin, _inPinID);

            InitializePin(gpio, out _outPin, _outPinID);
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

        private void InitializePin(GpioController gpioController, out GpioPin pin, int pinId)
        {
            pin = gpioController.OpenPin(pinId);
            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        /// <summary>
        /// Timer function for Filling.  When the tick interval is reached, this function will run and set the 
        /// GPIO high (which closed the valves) and stops the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_fill_Tick(object sender, object e)
        {
            _inPin.Write(GpioPinValue.High);

            _fillPin.Write(GpioPinValue.High);

            _outPin.Write(GpioPinValue.High);

            _wastePin.Write(GpioPinValue.High);

            _timer_fill.Stop();

            _timer_pumpOffDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);

            OnFillDone(EventArgs.Empty);
        }

        /// <summary>
        /// Timer function for drain.  When the tick interval is reached, this function will run and set the 
        /// GPIO high (which closed the valves) and stops the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_drain_Tick(object sender, object e)
        {
            _inPin.Write(GpioPinValue.High);

            _fillPin.Write(GpioPinValue.High);

            _outPin.Write(GpioPinValue.High);

            _wastePin.Write(GpioPinValue.High);

            _timer_drain.Stop();

            _timer_pumpOffDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);

            OnDrainDone(EventArgs.Empty);
        }

        /// <summary>
        /// Timer function for turning the pump on.  When the tick interval is reached, this function will run and set the 
        /// <see cref="_pumpPin"/> low (which closed turns it on)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_pumpOnDelay_Tick(object sender)
        {
            _pumppinValue = GpioPinValue.Low;
            _pumpPin.Write(_pumppinValue);

            _timer_pumpOnDelay.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Timer function for turning the pump on.  When the tick interval is reached, this function will run and set the 
        /// <see cref="_pumpPin"/> low (which closed turns it on)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_pumpOffDelay_Tick(object sender)
        {
            _pumppinValue = GpioPinValue.High;
            _pumpPin.Write(_pumppinValue);

            _timer_pumpOffDelay.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// GUI click event for filling.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valve.
        /// </summary>
        /// <returns>The value of the <see cref="_pumppinValue"/></returns>
        public GpioPinValue Fill()
        {
            _timer_fill.Start();
            
            if (_pumppinValue == GpioPinValue.High)
            {
                _inPin.Write(GpioPinValue.Low);

                _fillPin.Write(GpioPinValue.Low);

                _outPin.Write(GpioPinValue.High);

                _wastePin.Write(GpioPinValue.High);

                _timer_pumpOnDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
            }
            return _pumppinValue;
        }

        /// <summary>
        /// GUI click event for draining.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valves.
        /// </summary>
        /// <returns>The value of the <see cref="_pumppinValue"/></returns>
        public GpioPinValue Drain()
        {
            _timer_drain.Start();
            if (_pumppinValue == GpioPinValue.High)
            {
                _outPin.Write(GpioPinValue.Low);

                _wastePin.Write(GpioPinValue.Low);

                _inPin.Write(GpioPinValue.High);

                _fillPin.Write(GpioPinValue.High);

                _timer_pumpOnDelay.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
            }
            return _pumppinValue;
        }
        #endregion
    }
}
