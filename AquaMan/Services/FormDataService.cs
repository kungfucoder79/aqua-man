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
        public FeedingTimes GetFeedingTimes()
        {
            return new FeedingTimes();
        }

        /// <summary>
        /// Sets the feeding times
        /// </summary>
        /// <returns></returns>
        public void SetFeedingTimes(FeedingTimes feedingTimes)
        {
            //Save more stuffs
        }

        /// <summary>
        /// Gets the tank size specs, <see cref="TankSpecs"/>
        /// </summary>
        public TankSpecs GetTankSpecs()
        {
            TankSpecs tankSpecs = new TankSpecs()
            {
                Initialized = false,
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
