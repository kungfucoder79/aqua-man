using AquaMan.Models;
using System;
using System.Collections.Generic;

namespace AquaMan.Services
{
    /// <summary>
    /// Implementation of the data layer
    /// </summary>
    public class FormDataService : IFormDataService
    {
        /// <summary>
        /// Gets the feeding times
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> GetFeedingTimes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the feeding times
        /// </summary>
        /// <returns></returns>
        public void SetFeedingTimes(IEnumerable<DateTime> dateTimes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the tank size specs, <see cref="TankSpecs"/>
        /// </summary>
        public TankSpecs GetTankSpecs()
        {
            TankSpecs tankSpecs = new TankSpecs()
            {
                Height = 6,
                Width = 12,
                Depth = 22,
            };
            return tankSpecs;
        }
        
        /// <summary>
        /// Sets the tank size specs, <see cref="TankSpecs"/>
        /// </summary>
        /// <param name="tankSpecs"></param>
        public void SetTankSpecs(TankSpecs tankSpecs)
        {
            //save stuff
        }
    }
}
