using System;
using System.Collections;
using System.Collections.Generic;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public interface IAquaPinController
    {
        /// <summary>
        /// Gets if the pump and valves are currently running a fill sequence
        /// </summary>
        bool IsFillActive { get; }

        /// <summary>
        /// Gets if the pump and valves are currently running a drain sequence
        /// </summary>
        bool IsDrainActive { get; }

        /// <summary>
        /// Gets if the pump is currently active
        /// </summary>
        bool IsPumpActive { get; }
        
        /// <summary>
        /// Update the list of feeding times
        /// </summary>
        /// <param name="_feedingTimes"></param>
        void UpdateFeedingTimes(IEnumerable<DateTime?> _feedingTimes, int pinches);

        /// <summary>
        /// Starts the Drain sequence
        /// </summary>
        void Drain();

        /// <summary>
        /// Shuts off valves and pump
        /// </summary>
        void Stop();

        /// <summary>
        /// Starts the Fill salt water sequence
        /// </summary>
        void FillSaltWater();

        /// <summary>
        /// Starts the Fill fresh water
        /// </summary>
        void FillFreshWater();

        /// <summary>
        /// Starts the Feed sequence
        /// </summary>
        void Feed();

        /// <summary>
        /// Toggles the output valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        bool ToggleValve1();

        /// <summary>
        /// Toggles the input valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        bool ToggleValve2();

        /// <summary>
        /// Toggles the fresh water valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        bool ToggleValve3();

        /// <summary>
        /// Toggles the salt water valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        bool ToggleValve4();

        /// <summary>
        /// Toggles the drain valve
        /// </summary>
        /// <returns>the state of the valve</returns>
        bool ToggleValve5();
    }
}