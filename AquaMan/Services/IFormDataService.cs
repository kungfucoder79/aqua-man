using Microsoft.AspNetCore.Mvc.Rendering;
using AquaMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Services
{
    /// <summary>
    /// Interface for getting form data 
    /// </summary>
    public interface IFormDataService
    {
        /// <summary>
        /// Gets the feeding times
        /// </summary>
        /// <returns></returns>
        IEnumerable<DateTime> GetFeedingTimes();

        /// <summary>
        /// Gets the feeding times
        /// </summary>
        /// <returns></returns>
        void SetFeedingTimes(IEnumerable<DateTime> dateTimes);

        /// <summary>
        /// Gets the tank size specs, <see cref="TankSpecs"/>
        /// </summary>
        TankSpecs GetTankSpecs();

        /// <summary>
        /// Sets the tank size specs, <see cref="TankSpecs"/>
        /// </summary>
        /// <param name="tankSpecs"></param>
        void SetTankSpecs(TankSpecs tankSpecs);

        
    }
}
