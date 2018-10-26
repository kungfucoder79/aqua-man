using System;
using System.Collections;
using System.Collections.Generic;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public interface IAquaPinController
    {
        void UpdateFeedingTimes(IEnumerable<DateTime?> _feedingTimes);

        void Drain();

        void Fill();

        void Feed();
    }
}