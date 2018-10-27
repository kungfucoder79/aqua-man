using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aqua_Control
{
    public class PinMasterController
    {
        #region Members
        private IAquaI2CController _aquaI2CController;
        private IAquaPinController _aquaPinController;
        private Timer _waterLevelTimer;
        private static TimeSpan _waterLevelCheckInterval = TimeSpan.FromSeconds(1);
        private double _waterHeight;
        private DateTime _waterChangeTime;
        #endregion

        #region ctor
        public PinMasterController(IAquaI2CController aquaI2CController, IAquaPinController aquaPinController)
        {
            _aquaI2CController = aquaI2CController;
            _aquaPinController = aquaPinController;
            _waterLevelTimer = new Timer(CheckWaterLevel, null, TimeSpan.Zero, _waterLevelCheckInterval);
            _waterChangeTime = new DateTime();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Checks the water level of the tank and decides if it need to fill
        /// </summary>
        /// <param name="state"></param>
        private void CheckWaterLevel(object state)
        {
            _waterHeight = _aquaI2CController.GetWaterHeight();
            if (_waterHeight <= 4 && !_aquaPinController.IsPumpActive)
            {
                _aquaPinController.Fill();
            }
        }
        #endregion
    }
}
