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
    public class EmptyPinController : BasePinController, IAquaPinController
    {
        #region ctor
        /// <summary>
        /// Constructs a new <see cref="AquaGPIO"/> object by initialzing the gpio pins for the raspberry pi
        /// </summary>
        public EmptyPinController(IEnumerable<DateTime?> feedingTimes, int pinches)
            : base(feedingTimes, pinches)
        {
            Console.WriteLine($"--------------{DateTime.Now}: Initializing Pin Controller");
        }

        #endregion

        #region Methods
        protected override void _timerController_FeederStart(object sender, EventArgs e)
        {
            Feed();
        }

        protected override void _timerController_PumpOn(object sender, EventArgs e)
        {
            Console.WriteLine($"PUMP----------------{DateTime.Now}: TURNING PUMP ON");
        }

        protected override void _timerController_PumpOff(object sender, EventArgs e)
        {
            Console.WriteLine($"PUMP----------------{DateTime.Now}: TURNING PUMP OFF");
            IsFillActive = false;
            IsDrainActive = false;
        }

        /// <summary>
        /// Set the pins to high which turns the valves off
        /// </summary>
        protected override void TurnValvesOff()
        {
            Console.WriteLine($"----------------{DateTime.Now}: TURNING VALVE OFF");
        }

        /// <summary>
        /// Starts the Fill salt water
        /// </summary>
        public void FillSaltWater()
        {
            IsFillActive = true;
            Console.WriteLine($"----------------{DateTime.Now}: FILL SALT ON");
            TimerController.SetPumpOnDelay();
        }

        /// <summary>
        /// Starts the Fill fresh water
        /// </summary>
        public void FillFreshWater()
        {
            IsFillActive = true;
            Console.WriteLine($"----------------{DateTime.Now}: FILL FRESH ON");
            TimerController.SetPumpOnDelay();
        }

        /// <summary>
        /// GUI click event for draining.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valves.
        /// </summary>
        public void Drain()
        {
            IsDrainActive = true;
            Console.WriteLine($"----------------{DateTime.Now}: DRAIN ON");
            TimerController.SetPumpOnDelay();
        }

        /// <summary>
        /// Starts the feeding auger sequence with the specified number of times to step.
        /// </summary>
        /// <param name="numberOfTimes"></param>
        public async void Feed()
        {
            Console.WriteLine($"----------------{DateTime.Now}: FEEDING STARTED");

            await Task.Delay(300);

            Console.WriteLine($"----------------{DateTime.Now}: FEEDING STOPPED");
        }

        /// <summary>
        /// Toggles the output valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        public bool ToggleValve1()
        {
            _V1Latch = !_V1Latch;
            Console.WriteLine($"----------------{DateTime.Now}: {nameof(ToggleValve1)}: {_V1Latch}");
            return _V1Latch;
        }

        /// <summary>
        /// Toggles the input valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        public bool ToggleValve2()
        {
            _V2Latch = !_V2Latch;
            Console.WriteLine($"----------------{DateTime.Now}: {nameof(ToggleValve2)}: {_V2Latch}");
            return _V2Latch;
        }

        /// <summary>
        /// Toggles the fresh water valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        public bool ToggleValve3()
        {
            _V3Latch = !_V3Latch;
            Console.WriteLine($"----------------{DateTime.Now}: {nameof(ToggleValve3)}: {_V3Latch}");
            return _V3Latch;
        }

        /// <summary>
        /// Toggles the salt water valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        public bool ToggleValve4()
        {
            _V4Latch = !_V4Latch;
            Console.WriteLine($"----------------{DateTime.Now}: {nameof(ToggleValve4)}: {_V4Latch}");
            return _V4Latch;
        }

        /// <summary>
        /// Toggles the drain valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        public bool ToggleValve5()
        {
            _V5Latch = !_V5Latch;
            Console.WriteLine($"----------------{DateTime.Now}: {nameof(ToggleValve5)}: {_V5Latch}");
            return _V5Latch;
        }
        #endregion
    }
}
