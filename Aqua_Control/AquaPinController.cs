using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public class AquaPinController : BasePinController, IAquaPinController
    {
        #region Members
        //the gpio pins and their respective pin numbers on the board

        //Valve 1
        private GpioPin _outPin_Open;

        private GpioPin _outPin_Close;
        
        //Valve 2
        private GpioPin _inPin_Open;

        private GpioPin _inPin_Close;

        //Valve 3
        private GpioPin _fillFreshPin_Open;

        private GpioPin _fillFreshPin_Close;

        //Valve 4
        private GpioPin _fillSaltPin_Open;

        private GpioPin _fillSaltPin_Close;
        
        //Valve 5
        private GpioPin _wastePin_Open;

        private GpioPin _wastePin_Close;

        private GpioPin _pumpPin_Open;

        private GpioPin _feedPin1;

        private GpioPin _feedPin2;

        private GpioPin _feedPin3;

        private GpioPin _feedPin4;

        private const GpioPinValue _0 = GpioPinValue.Low;
        private const GpioPinValue _1 = GpioPinValue.High;

        private GpioPinValue[][] _feederSequences = new GpioPinValue[][]{
            new[] {_0, _1, _1, _1},
            new[] {_1, _0, _1, _1},
            new[] {_1, _1, _0, _1},
            new[] {_1, _1, _1, _0}};
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new <see cref="AquaGPIO"/> object by initialzing the gpio pins for the raspberry pi
        /// </summary>
        public AquaPinController(IEnumerable<DateTime?> feedingTimes)
            :base(feedingTimes)
        {
            GpioController gpioController = GpioController.Instance;

            // Below are the separate pins being used from the raspberry pi and how they are setup
            // need to assign pins, values, and driver output

            _pumpPin_Open = InitializePin(gpioController[P1.Gpio26], _0, GpioPinDriveMode.Output);

            _fillSaltPin_Open = InitializePin(gpioController[P1.Gpio10], _0, GpioPinDriveMode.Output);
            _fillSaltPin_Close = InitializePin(gpioController[P1.Gpio22], _1, GpioPinDriveMode.Output);

            _fillFreshPin_Open = InitializePin(gpioController[P1.Gpio11], _0, GpioPinDriveMode.Output);
            _fillFreshPin_Close = InitializePin(gpioController[P1.Gpio09], _1, GpioPinDriveMode.Output);

            _inPin_Open = InitializePin(gpioController[P1.Gpio06], _0, GpioPinDriveMode.Output);
            _inPin_Close = InitializePin(gpioController[P1.Gpio05], _1, GpioPinDriveMode.Output);

            _wastePin_Open = InitializePin(gpioController[P1.Gpio27], _0, GpioPinDriveMode.Output);
            _wastePin_Close = InitializePin(gpioController[P1.Gpio17], _1, GpioPinDriveMode.Output);

            _outPin_Open = InitializePin(gpioController[P1.Gpio19], _0, GpioPinDriveMode.Output);
            _outPin_Close = InitializePin(gpioController[P1.Gpio13], _1, GpioPinDriveMode.Output);

            _feedPin1 = InitializePin(gpioController[P1.Gpio23], _0, GpioPinDriveMode.Output);
            _feedPin2 = InitializePin(gpioController[P1.Gpio24], _0, GpioPinDriveMode.Output);
            _feedPin3 = InitializePin(gpioController[P1.Gpio25], _0, GpioPinDriveMode.Output);
            _feedPin4 = InitializePin(gpioController[P1.Gpio08], _0, GpioPinDriveMode.Output);
        }

        protected override void _timerController_FeederStart(object sender, EventArgs e)
        {
            Feed();
        }

        protected override void _timerController_PumpOn(object sender, EventArgs e)
        {
            _pumpPin_Open.Write(_1);
        }

        protected override void _timerController_PumpOff(object sender, EventArgs e)
        {
            _pumpPin_Open.Write(_0);
            IsPumpActive = false;
        }

        #endregion

        #region Methods
        private GpioPin InitializePin(GpioPin pin, GpioPinValue gpioPinValue, GpioPinDriveMode gpioPinDriveMode)
        {
            pin.PinMode = gpioPinDriveMode;
            pin.Write(gpioPinValue);
            return pin;
        }

        /// <summary>
        /// Set the pins to high which turns the valves off
        /// </summary>
        protected override void TurnValvesOff()
        {
            _inPin_Open.Write(_0);
            _inPin_Close.Write(_1);

            _fillSaltPin_Open.Write(_0);
            _fillSaltPin_Close.Write(_1);

            _outPin_Open.Write(_0);
            _outPin_Close.Write(_1);

            _wastePin_Open.Write(_0);
            _wastePin_Close.Write(_1);
        }

        /// <summary>
        /// Initicates the fill sequence. Will start the associated timer and set the GPIO pins.
        /// to turn on the correct valves.
        /// </summary>
        public void Fill()
        {
            IsPumpActive = true;

            _inPin_Open.Write(_1);
            _inPin_Close.Write(_0);

            _fillSaltPin_Open.Write(_1);
            _fillSaltPin_Close.Write(_0);

            _outPin_Open.Write(_0);
            _outPin_Close.Write(_1);

            _wastePin_Open.Write(_0);
            _wastePin_Close.Write(_1);

            TimerController.SetPumpOnDelay();
        }

        /// <summary>
        /// Initicates the drain sequence. Will start the associated timer and set the GPIO pins.
        /// to turn on the correct valves.
        /// </summary>
        public void Drain()
        {
            IsPumpActive = true;

            _outPin_Open.Write(_1);
            _outPin_Close.Write(_0);

            _wastePin_Open.Write(_1);
            _wastePin_Close.Write(_0);

            _inPin_Open.Write(_0);
            _inPin_Close.Write(_1);

            _fillSaltPin_Open.Write(_0);
            _fillSaltPin_Close.Write(_1);

            TimerController.SetPumpOnDelay();
        }

        /// <summary>
        /// Starts the feeding auger sequence.
        /// </summary>
        public async void Feed()
        {
            int numberOfTimes = 40;
            int delay = 1;
            for (int i = 0; i < numberOfTimes; i++)
            {
                for (int j = 0; j < _feederSequences[0].Length; j++)
                {
                    _feedPin1.Write(_feederSequences[j][0]);
                    _feedPin2.Write(_feederSequences[j][1]);
                    _feedPin3.Write(_feederSequences[j][2]);
                    _feedPin4.Write(_feederSequences[j][3]);
                    await Task.Delay(delay);
                }
            }
        }
        #endregion
    }
}
