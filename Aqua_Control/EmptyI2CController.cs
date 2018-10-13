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
        public EmptyI2CController()
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
            Console.WriteLine($"----------------{DateTime.Now}: {nameof(Reset)}");
        }


        public double GetWaterHeight()
        {
            Console.WriteLine($"----------------{DateTime.Now}: {nameof(GetWaterHeight)}");
            return 0.0;
        }

        public void InitI2C()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
