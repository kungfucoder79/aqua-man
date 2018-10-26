using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Models
{
    /// <summary>
    /// Class for the feeding times
    /// </summary>
    public class FeedingTimes
    {
        public FeedingTimes()
        {
            Feedings = new List<DateTime?>();
        }
        /// <summary>
        /// A list of times to initiate the feeding sequence
        /// </summary>
        public List<DateTime?> Feedings { get; private set; }
    }
}
