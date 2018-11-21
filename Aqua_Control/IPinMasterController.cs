using System;

namespace Aqua_Control
{
    /// <summary>
    /// Interface for the water change sequeces
    /// </summary>
    public interface IPinMasterController
    {
        /// <summary>
        /// Updates the time for the water change sequence
        /// </summary>
        /// <param name="waterChangeTime"></param>
        void UpdateWaterChangeTime(DateTime waterChangeTime);
        /// <summary>
        /// Manual override for water change
        /// </summary>
        void WaterChange();
    }
}