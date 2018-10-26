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
    public class EmptyPinController : IAquaPinController
    {
        #region Members
        private TimerController _timerController;
        #endregion

        #region ctor
        /// <summary>
        /// Constructs a new <see cref="AquaGPIO"/> object by initialzing the gpio pins for the raspberry pi
        /// </summary>
        public EmptyPinController()
        {
            Console.WriteLine($"--------------{DateTime.Now}: Initializing Pin Controller");
            _timerController = new TimerController();
            _timerController.DrainDone += _timerController_DrainDone;
            _timerController.FillDone += _timerController_FillDone;
            _timerController.PumpOff += _timerController_PumpOff;
            _timerController.PumpOn += _timerController_PumpOn;
            _timerController.FeederStart += _timerController_FeederStart;
        }

        #endregion

        #region Methods
        private void _timerController_FeederStart(object sender, EventArgs e)
        {
            FeedMe(20);
        }

        private void _timerController_PumpOn(object sender, EventArgs e)
        {
            Console.WriteLine($"PUMP----------------{DateTime.Now}: TURNING PUMP ON");
        }

        private void _timerController_PumpOff(object sender, EventArgs e)
        {
            Console.WriteLine($"PUMP----------------{DateTime.Now}: TURNING PUMP OFF");
        }

        private void _timerController_FillDone(object sender, EventArgs e)
        {
            TurnValvesOff();
        }

        private void _timerController_DrainDone(object sender, EventArgs e)
        {
            TurnValvesOff();
        }
        /// <summary>
        /// Set the pins to high which turns the valves off
        /// </summary>
        private void TurnValvesOff()
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
            Console.WriteLine($"----------------{DateTime.Now}: FILL ON");
            _timerController.SetFillTimer();
        }

        /// <summary>
        /// GUI click event for draining.  Will start the associated timer and set the selected GPIO pins LOW
        /// to turn on the correct valves.
        /// </summary>
        /// <returns>The value of the <see cref="_pumppinValue"/></returns>
        public void Drain()
        {
            Console.WriteLine($"----------------{DateTime.Now}: DRAIN ON");
            _timerController.SetDrainTimer();
        }

        /// <summary>
        /// Starts the feeding auger sequence with the specified number of times to step.
        /// </summary>
        /// <param name="numberOfTimes"></param>
        public async void FeedMe(int numberOfTimes)
        {
            Console.WriteLine($"----------------{DateTime.Now}: FEEDING STARTED");
            int delay = 1;
            for (int i = 0; i < numberOfTimes; i++)
            {
                Console.WriteLine($"FEEDING----------------{DateTime.Now}: {i}");
                await Task.Delay(delay);
            }
            Console.WriteLine($"----------------{DateTime.Now}: FEEDING STOPPED");
        }
        #endregion
    }
}
