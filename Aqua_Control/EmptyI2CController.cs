using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public class EmptyI2CController : IAquaI2CController
    {
        #region ctor
        /// <summary>
        /// Creates a new <see cref="AquaI2C"/>
        /// </summary>
        public EmptyI2CController(double tankWidth, double tankHeight, double tankDepth)
        {
        }
        #endregion

        #region Properties
        public I2CDevice I2CSensor { get; private set; }
        public float FinalCapMeasure1 { get; private set; }
        public float FinalCapMeasure2 { get; private set; }
        public float FinalCapMeasure3 { get; private set; }
        public OneRegisterRead DataIn { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Resets the I2C port
        /// </summary>
        public void Reset()
        {
            Console.WriteLine($"WTR----------------{DateTime.Now}: {nameof(Reset)}");
        }

        Random random = new Random();
        public double GetWaterHeight()
        {
            double testVal = random.Next(0, 5);
            Console.WriteLine($"WTR----------------{DateTime.Now}: {testVal}");
            return testVal;
        }

        public void CalabrateSensor()
        {
            Console.WriteLine($"{nameof(CalabrateSensor)}");
        }

        public void UpdateTankSpecs(double tankWidth, double tankHeight, double tankDepth)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
