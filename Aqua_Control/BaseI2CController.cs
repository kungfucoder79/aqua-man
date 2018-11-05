using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Aqua_Control
{
    /// <summary>
    /// Base class for I2C logic
    /// </summary>
    public abstract class BaseI2CController
    {
        #region Members
        private readonly Timer _timer;
        private static int queLength = 100;
        private int queCount = 1;
        private Queue<double> avg = new Queue<double>();
        protected bool _calabrate = false;
        private double _TankHeight;
        private double _TankWidth;
        private double _TankDepth;
        #endregion

        #region ctor
        /// <summary>
        /// Creates a new <see cref="BaseI2CController"/>
        /// </summary>
        public BaseI2CController(double tankWidth, double tankHeight, double tankDepth)
        {
            _timer = new Timer(I2CCheck, null, 0, 25);


            _TankHeight = tankHeight;
            _TankWidth = tankWidth;
            _TankDepth = tankDepth;
        }

        #endregion

        #region Methods

        private void I2CCheck(object state)
        {
            double waterHeight = GetWaterHeight();
            //Console.WriteLine($"{nameof(waterHeight)} = {waterHeight}");
        }

        public abstract double GetWaterHeight();

        protected double Average(double testVal)
        {
            if (queCount <= queLength)
            {
                queCount++;
            }
            else
            {
                avg.Dequeue();
            }

            avg.Enqueue(testVal);
            double avgVal = avg.Sum() / avg.Count;
            return avgVal;
        }

        /// <summary>
        /// Updates the tank specification from the user
        /// </summary>
        /// <param name="tankWidth"></param>
        /// <param name="tankHeight"></param>
        /// <param name="tankDepth"></param>
        public void UpdateTankSpecs(double tankWidth, double tankHeight, double tankDepth)
        {
            _TankHeight = tankHeight;
            _TankWidth = tankWidth;
            _TankDepth = tankDepth;
        }
        #endregion
    }
}
