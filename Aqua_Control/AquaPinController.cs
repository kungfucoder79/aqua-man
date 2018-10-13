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

        private const GpioPinValue _0 = GpioPinValue.Low;
        private const GpioPinValue _1 = GpioPinValue.High;

        private GpioPinValue[][] _feederSequences = new GpioPinValue[][]{
            new[] {_0, _1, _1, _1},
            new[] {_1, _0, _1, _1},
            new[] {_1, _1, _0, _1},
            new[] {_1, _1, _1, _0}};

        private TimerController _timerController;
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new <see cref="AquaGPIO"/> object by initialzing the gpio pins for the raspberry pi
        /// </summary>
        public AquaPinController()
        {
            _timerController = new TimerController();
            _timerController.DrainDone += _timerController_DrainDone;
            _timerController.FillDone += _timerController_FillDone;
            _timerController.PumpOff += _timerController_PumpOff;
            _timerController.PumpOn += _timerController_PumpOn;


            GpioController gpioController = GpioController.Instance;

            // Below are the separate pins being used from the raspberry pi and how they are setup
            // need to assign pins, values, and driver output

            _pumpPin = InitializePin(gpioController[P1.Gpio16], _0, GpioPinDriveMode.Output);

            _fillPin = InitializePin(gpioController[P1.Gpio20], _0, GpioPinDriveMode.Output);

            _wastePin = InitializePin(gpioController[P1.Gpio21], _0, GpioPinDriveMode.Output);

            _inPin = InitializePin(gpioController[P1.Gpio19], _0, GpioPinDriveMode.Output);

            _outPin = InitializePin(gpioController[P1.Gpio26], _0, GpioPinDriveMode.Output);

            _inPin1 = InitializePin(gpioController[P1.Gpio23], _0, GpioPinDriveMode.Output);

            _inPin2 = InitializePin(gpioController[P1.Gpio24], _0, GpioPinDriveMode.Output);

            _inPin3 = InitializePin(gpioController[P1.Gpio25], _0, GpioPinDriveMode.Output);

            _inPin4 = InitializePin(gpioController[P1.Gpio08], _0, GpioPinDriveMode.Output);
        }

        private void _timerController_PumpOn(object sender, EventArgs e)
        {
            _pumpPin.Write(_1);
        }

        private void _timerController_PumpOff(object sender, EventArgs e)
        {
            _pumpPin.Write(_0);
        }

        private void _timerController_FillDone(object sender, EventArgs e)
        {
            TurnValvesOff();
        }

        private void _timerController_DrainDone(object sender, EventArgs e)
        {
            TurnValvesOff();
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
        private void TurnValvesOff()
        {
            _inPin.Write(_0);

            _fillPin.Write(_0);

            _outPin.Write(_0);

            _wastePin.Write(_0);
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

            _timerController.SetFillTimer();
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

            _timerController.SetDrainTimer();
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
