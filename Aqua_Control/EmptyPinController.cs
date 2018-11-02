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
        public EmptyPinController(IEnumerable<DateTime?> feedingTimes)
            : base(feedingTimes)
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
            IsPumpActive = false;
        }

        /// <summary>
        /// Set the pins to high which turns the valves off
        /// </summary>
        protected override void TurnValvesOff()
        {
            Console.WriteLine($"----------------{DateTime.Now}: TURNING VALVE OFF");
        }

        /// <summary>
        /// GUI click event for filling.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valve.
        /// </summary>
        /// <returns>The value of the <see cref="_pumppinValue"/></returns>
        public void Fill()
        {
            IsPumpActive = true;
            Console.WriteLine($"----------------{DateTime.Now}: FILL ON");
            TimerController.SetPumpOnDelay();
        }

        /// <summary>
        /// GUI click event for draining.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valves.
        /// </summary>
        /// <returns>The value of the <see cref="_pumppinValue"/></returns>
        public void Drain()
        {
            IsPumpActive = true;
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
        #endregion
    }
}
