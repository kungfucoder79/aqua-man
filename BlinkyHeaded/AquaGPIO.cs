using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
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
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new <see cref="AquaGPIO"/> object by initialzing the gpio pins for the raspberry pi
        /// </summary>
        public AquaGPIO()
        {
            _timer_fill = new DispatcherTimer();
            _timer_fill.Interval = TimeSpan.FromSeconds(1);
            _timer_fill.Tick += Timer_fill_Tick;

            _timer_drain = new DispatcherTimer();
            _timer_drain.Interval = TimeSpan.FromSeconds(1);
            _timer_drain.Tick += Timer_drain_Tick;

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


        #region Methods
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
            _pumppinValue = GpioPinValue.High;
            _pumpPin.Write(_pumppinValue);

            _inPin.Write(GpioPinValue.High);

            _fillPin.Write(GpioPinValue.High);

            _outPin.Write(GpioPinValue.High);

            _wastePin.Write(GpioPinValue.High);

            _timer_fill.Stop();
        }

        /// <summary>
        /// Timer function for drain.  When the tick interval is reached, this function will run and set the 
        /// GPIO high (which closed the valves) and stops the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_drain_Tick(object sender, object e)
        {
            _pumppinValue = GpioPinValue.High;
            _pumpPin.Write(_pumppinValue);

            _inPin.Write(GpioPinValue.High);

            _fillPin.Write(GpioPinValue.High);

            _outPin.Write(GpioPinValue.High);

            _wastePin.Write(GpioPinValue.High);

            _timer_drain.Stop();
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
                _pumppinValue = GpioPinValue.Low;
                _pumpPin.Write(_pumppinValue);

                _inPin.Write(GpioPinValue.Low);

                _fillPin.Write(GpioPinValue.Low);

                _outPin.Write(GpioPinValue.High);

                _wastePin.Write(GpioPinValue.High);
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
                _pumppinValue = GpioPinValue.Low;
                _pumpPin.Write(_pumppinValue);

                _outPin.Write(GpioPinValue.Low);

                _wastePin.Write(GpioPinValue.Low);

                _inPin.Write(GpioPinValue.High);

                _fillPin.Write(GpioPinValue.High);
            }
            return _pumppinValue;
        }
        #endregion
    }
}
