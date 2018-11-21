using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aqua_Control
{
    /// <summary>
    /// Contains the logic for firing sequences in the <see cref="IAquaPinController"/> based on reading from the <see cref="IAquaI2CController"/>
    /// </summary>
    public class PinMasterController : IPinMasterController
    {
        #region Members
        private IAquaI2CController _aquaI2CController;
        private IAquaPinController _aquaPinController;
        private Timer _waterLevelTimer;
        private Timer _waterChangeTimer;
        private static TimeSpan _waterLevelCheckInterval = TimeSpan.FromMilliseconds(250);
        private double _waterHeight;
        private DateTime _waterChangeTime;
        private bool _isChangeDrainDone;
        #endregion

        #region ctor
        /// <summary>
        /// Creates a new <see cref="PinMasterController"/>
        /// </summary>
        /// <param name="aquaI2CController"></param>
        /// <param name="aquaPinController"></param>
        public PinMasterController(IAquaI2CController aquaI2CController, IAquaPinController aquaPinController)
        {
            _aquaI2CController = aquaI2CController;
            _aquaPinController = aquaPinController;
            _waterLevelTimer = new Timer(CheckTopOff, null, TimeSpan.Zero, _waterLevelCheckInterval);
            _waterChangeTimer = new Timer(CheckWaterChangeLevel, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _isChangeDrainDone = false;
            _waterChangeTime = new DateTime();
        }

        private void CheckWaterChangeLevel(object state)
        {
            if(_aquaI2CController.WaterHeight < _aquaI2CController.Delta10Height && _isChangeDrainDone == false)
            {
                _isChangeDrainDone = true;
                _aquaPinController.Stop();
                Console.WriteLine("Water Changed: Auto Stop Fired");
                Console.WriteLine($"Water Changed: {_aquaI2CController.WaterHeight}");
                Console.WriteLine($"Water Changed: {_aquaI2CController.Delta10Height}");
            }
            else if (_aquaPinController.IsPumpActive == false && _aquaPinController.IsFillActive == false && _aquaPinController.IsDrainActive == false)
            {
                _aquaPinController.FillSaltWater();
                Console.WriteLine("Water Changed: Auto fill Fired");
                Console.WriteLine($"Water Changed: {_aquaI2CController.WaterHeight}");
                Console.WriteLine($"Water Changed: {_aquaI2CController.Delta10Height}");
            }
            else if (_aquaI2CController.WaterHeight >= _aquaI2CController.TopWaterHeight && _aquaPinController.IsFillActive)
            {
                _aquaPinController.Stop();
                Console.WriteLine("Water Changed: WaterChange Off");
                _waterChangeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _waterLevelTimer.Change(TimeSpan.Zero, _waterLevelCheckInterval);
                _isChangeDrainDone = false;
                
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks the water level of the tank and decides it need to stop the fill sequence
        /// </summary>
        /// <param name="state"></param>
        private void CheckTopOff(object state)
        {
            _waterHeight = _aquaI2CController.WaterHeight;
            if (_aquaI2CController.IsTopSet)
            {
                if (_waterHeight >= _aquaI2CController.TopWaterHeight && _aquaPinController.IsFillActive)
                {
                    _aquaPinController.Stop();
                    Console.WriteLine("Auto Stop Fired");
                }
                if (_waterHeight < _aquaI2CController.Delta5Height && _aquaPinController.IsFillActive == false && _aquaPinController.IsDrainActive == false)
                {
                    _aquaPinController.FillSaltWater();
                    Console.WriteLine($"{nameof(_aquaPinController.IsFillActive)} = {_aquaPinController.IsFillActive}");
                    Console.WriteLine($"{nameof(_aquaI2CController.Delta5Height)} = {_aquaI2CController.Delta5Height}");
                    Console.WriteLine($"{nameof(_aquaI2CController.WaterHeight)} = {_aquaI2CController.WaterHeight}");
                }
            }
        }
        
        public void WaterChange()
        {
            _waterLevelTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _aquaPinController.Drain();
            _waterChangeTimer.Change(TimeSpan.Zero, _waterLevelCheckInterval);
        }
        #endregion
    }
}
