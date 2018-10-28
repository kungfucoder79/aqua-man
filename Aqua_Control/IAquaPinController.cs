﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public interface IAquaPinController
    {
        /// <summary>
        /// Gets if the pump and valves are currently running a fill or drain sequence
        /// </summary>
        bool IsPumpActive { get; }

        /// <summary>
        /// Update the list of feeding times
        /// </summary>
        /// <param name="_feedingTimes"></param>
        void UpdateFeedingTimes(IEnumerable<DateTime?> _feedingTimes);

        /// <summary>
        /// Starts the Drain sequence
        /// </summary>
        void Drain();

        /// <summary>
        /// Shuts off valves and pump
        /// </summary>
        void Stop();

        /// <summary>
        /// Starts the Fill sequence
        /// </summary>
        void Fill();

        /// <summary>
        /// Starts the Feed sequence
        /// </summary>
        void Feed();
    }
}