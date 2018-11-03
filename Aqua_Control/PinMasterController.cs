using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aqua_Control
{
    /// <summary>
    /// Contains the logic for firing sequences in the <see cref="IAquaPinController"/> based on reading from the <see cref="IAquaI2CController"/>
    /// </summary>
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
        /// <summary>
        /// Creates a new <see cref="PinMasterController"/>
        /// </summary>
        /// <param name="aquaI2CController"></param>
        /// <param name="aquaPinController"></param>
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
        /// Checks the water level of the tank and decides it need to stop the fill sequence
        /// </summary>
        /// <param name="state"></param>
        private void CheckWaterLevel(object state)
        {
            _waterHeight = _aquaI2CController.GetWaterHeight();
            if (_waterHeight >= 4 && _aquaPinController.IsPumpActive)
            {
                _aquaPinController.Stop();
            }
        }
        #endregion
    }
}
