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
        private static int _readingInterval = 50;
        private static int _queLength = 100;
        private int _queCount = 1;
        private int _readCount = 1;
        private Queue<double> _avg = new Queue<double>();
        protected bool _calabrate = false;
        private double _TankHeight;
        private double _TankWidth;
        private double _TankDepth;
        private double _filterResult = 0.0;
        private readonly double _dampeningConstant = 0.05;
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
            _timer = new Timer(ReadSensor, null, 0, Timeout.Infinite);

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
        private void ReadSensor(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            GetWaterHeight();

            _timer.Change(_readingInterval, Timeout.Infinite);
        }

        /// <summary>
        /// Gets the height of the water from the I2C sensor
        /// </summary>
        public abstract void GetWaterHeight();

        /// <summary>
        /// Using a low pass filter to dampen the noise
        /// </summary>
        /// <param name="inputWaterHeight"></param>
        /// <returns></returns>
        protected double Filter(double inputWaterHeight)
        {
            _filterResult += _dampeningConstant * (inputWaterHeight - _filterResult);

            return _filterResult;
        }

        protected double Average(double testVal)
        {
            semaphoreSlim.WaitAsync();
            try
            {
                if (_queCount <= _queLength)
                {
                    _queCount++;
                }
                else
                {
                    _avg.Dequeue();
                }
                if (_readCount > 1000)
                {
                    _readCount = 0;
                    _queCount = 1;
                    _avg.Clear();
                    _avg = new Queue<double>(_queLength);
                   // Console.WriteLine($"Cleared!!!!!");
                }
                _readCount++;
                _avg.Enqueue(testVal);
                double avgVal = _avg.Sum() / _avg.Count;
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
