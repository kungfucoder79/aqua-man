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
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly Timer _timer;
        private static int queLength = 100;
        private int queCount = 1;
        private int readCount = 1;
        private Queue<double> avg = new Queue<double>();
        protected bool _calabrate = false;
        private double _TankHeight;
        private double _TankWidth;
        private double _TankDepth;
        #endregion

        #region Propeties
        /// <summary>
        /// Gets the current water height read from the sensor
        /// </summary>
        public double WaterHeight { get; protected set; }

        /// <summary>
        /// Total volume of the tank
        /// </summary>
        public double TankVolume { get; private set; }


        /// <summary>
        /// Gets the change in height in the tank.
        /// </summary>
        public double DeltaHeight {
            get
            {
                return (TankVolume * .1) / (_TankDepth * _TankWidth);
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// Creates a new <see cref="BaseI2CController"/>
        /// </summary>
        public BaseI2CController(double tankWidth, double tankHeight, double tankDepth)
        {
            _timer = new Timer(GetWaterHeight, null, 0, 50);

            _TankHeight = tankHeight;
            _TankWidth = tankWidth;
            _TankDepth = tankDepth;
            TankVolume = CalulateVolume();
        }
        #endregion

        #region Methods
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
            TankVolume = CalulateVolume();
        }

        /// <summary>
        /// Gets the height of the water from the I2C sensor
        /// </summary>
        public abstract void GetWaterHeight(object state);

        protected double Average(double testVal)
        {
            semaphoreSlim.WaitAsync();
            try
            {
                if (queCount <= queLength)
                {
                    queCount++;
                }
                else
                {
                    avg.Dequeue();
                }
                if (readCount > 1000)
                {
                    readCount = 0;
                    queCount = 1;
                    avg.Clear();
                    avg = new Queue<double>(queLength);
                   // Console.WriteLine($"Cleared!!!!!");
                }
                readCount++;
                avg.Enqueue(testVal);
                double avgVal = avg.Sum() / avg.Count;
                return avgVal;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private double CalulateVolume()
        {
            return _TankDepth * _TankHeight * _TankWidth;
        }
        #endregion
    }
}
