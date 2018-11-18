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

        /// <summary>
        /// Wave
        /// </summary>
        private GpioPinValue[][] _feederSequences = new GpioPinValue[][]{
            new[] {_0, _1, _1, _1},
            new[] {_1, _0, _1, _1},
            new[] {_1, _1, _0, _1},
            new[] {_1, _1, _1, _0}};

        /// <summary>
        /// half step
        /// </summary>
        //private readonly GpioPinValue[][] _feederSequences =
        //{
        //    new[] {_0, _0, _1, _1, _1, _1, _1, _0},
        //    new[] {_1, _0, _0, _0, _1, _1, _1, _1},
        //    new[] {_1, _1, _1, _0, _0, _0, _1, _1},
        //    new[] {_1, _1, _1, _1, _1, _0, _0, _0 }};
        //private GpioPinValue[][] _feederSequences = new GpioPinValue[][]{
        //    new[] {_1, _1, _1, _0},
        //    new[] {_1, _1, _0, _1},
        //    new[] {_1, _0, _1, _1},
        //    new[] {_0, _1, _1, _1}};
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new <see cref="AquaGPIO"/> object by initialzing the gpio pins for the raspberry pi
        /// </summary>
        public AquaPinController(IEnumerable<DateTime?> feedingTimes, int pinches)
            :base(feedingTimes, pinches)
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

            _feedPin1 = InitializePin(gpioController[P1.Gpio21], _0, GpioPinDriveMode.Output);
            _feedPin2 = InitializePin(gpioController[P1.Gpio20], _0, GpioPinDriveMode.Output);
            _feedPin3 = InitializePin(gpioController[P1.Gpio16], _0, GpioPinDriveMode.Output);
            _feedPin4 = InitializePin(gpioController[P1.Gpio12], _0, GpioPinDriveMode.Output);
        }

        protected override void _timerController_FeederStart(object sender, EventArgs e)
        {
            Feed();
        }

        protected override void _timerController_PumpOn(object sender, EventArgs e)
        {
            _pumpPin_Open.Write(_1);
            IsPumpActive = true;
        }

        protected override void _timerController_PumpOff(object sender, EventArgs e)
        {
            _pumpPin_Open.Write(_0);
            IsFillActive = false;
            IsDrainActive = false;
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
            IsFillActive = false;
            IsDrainActive = false;
            CloseValve(_inPin_Open, _inPin_Close);

            CloseValve(_fillSaltPin_Open, _fillSaltPin_Close);

            CloseValve(_outPin_Open, _outPin_Close);

            CloseValve(_wastePin_Open, _wastePin_Close);
        }

        /// <summary>
        /// Initicates the fill sequence. Will start the associated timer and set the GPIO pins.
        /// to turn on the correct valves.
        /// </summary>
        public void FillSaltWater()
        {
            IsFillActive = true;

            OpenValve(_inPin_Open, _inPin_Close);

            OpenValve(_fillSaltPin_Open, _fillSaltPin_Close);

            CloseValve(_outPin_Open, _outPin_Close);

            CloseValve(_wastePin_Open, _wastePin_Close);

            TimerController.SetPumpOnDelay();
        }

        /// <summary>
        /// Initicates the drain sequence. Will start the associated timer and set the GPIO pins.
        /// to turn on the correct valves.
        /// </summary>
        public void Drain()
        {
            IsDrainActive = true;

            OpenValve(_outPin_Open, _outPin_Close);

            OpenValve(_wastePin_Open, _wastePin_Close);

            CloseValve(_inPin_Open, _inPin_Close);

            CloseValve(_fillSaltPin_Open, _fillSaltPin_Close);

            TimerController.SetPumpOnDelay();
        }

        /// <summary>
        /// Starts the feeding auger sequence.
        /// </summary>
        public async void Feed()
        {
            int numberOfTimes = 2000 * Pinches;
            TimeSpan delay = TimeSpan.FromMilliseconds(1);
            for (int i = 0; i < numberOfTimes; i++)
            {
                Console.WriteLine($"Feeding {i}");
                for (int j = 0; j < _feederSequences[0].Length; j++)
                {
                    _feedPin1.Write(_feederSequences[0][j]);
                    //Console.WriteLine($"Feeding {_feederSequences[j][0]}");
                    _feedPin2.Write(_feederSequences[1][j]);
                    //Console.WriteLine($"Feeding {_feederSequences[j][1]}");
                    _feedPin3.Write(_feederSequences[2][j]);
                    //Console.WriteLine($"Feeding {_feederSequences[j][2]}");
                    _feedPin4.Write(_feederSequences[3][j]);
                    //Console.WriteLine($"Feeding {_feederSequences[j][3]}");
                    await Task.Delay(delay);
                }
            }
        }

        public void ToggleValve1()
        {
            if(_V1Latch)
            {
                CloseValve(_outPin_Open, _outPin_Close);
            }
            else
            {
                OpenValve(_outPin_Open, _outPin_Close);
            }
            _V1Latch = !_V1Latch;
        }

        public void ToggleValve2()
        {
            if (_V2Latch)
            {
                CloseValve(_inPin_Open, _inPin_Close);
            }
            else
            {
                OpenValve(_inPin_Open, _inPin_Close);
            }
            _V2Latch = !_V2Latch;
        }

        public void ToggleValve3()
        {
            if (_V3Latch)
            {
                CloseValve(_fillFreshPin_Open, _fillFreshPin_Close);
            }
            else
            {
                OpenValve(_fillFreshPin_Open, _fillFreshPin_Close);
            }
            _V3Latch = !_V3Latch;
        }

        public void ToggleValve4()
        {
            if (_V4Latch)
            {
                CloseValve(_fillSaltPin_Open, _fillSaltPin_Close);
            }
            else
            {
                OpenValve(_fillSaltPin_Open, _fillSaltPin_Close);
            }
            _V4Latch = !_V4Latch;
        }

        public void ToggleValve5()
        {
            if (_V5Latch)
            {
                CloseValve(_wastePin_Open, _wastePin_Close);
            }
            else
            {
                OpenValve(_wastePin_Open, _wastePin_Close);
            }
            _V5Latch = !_V5Latch;
        }


        private void OpenValve(GpioPin openPin, GpioPin closePin)
        {
            openPin.Write(_1);
            closePin.Write(_0);
        }

        private void CloseValve(GpioPin openPin, GpioPin closePin)
        {
            openPin.Write(_0);
            closePin.Write(_1);
        }
        #endregion
    }
}
