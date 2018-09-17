using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Models
{
    /// <summary>
    /// Class for the feeding time
    /// </summary>
    public class FeedingTimes
    {
        /// <summary>
        /// First feeding time
        /// </summary>
        [Required(ErrorMessage = "At least one feeding time is required.")]
        public DateTime? Time1 { get; set; }

        /// <summary>
        /// Second feeding time
        /// </summary>
        public DateTime? Time2 { get; set; }

        /// <summary>
        /// Third feeding time
        /// </summary>
        public DateTime? Time3 { get; set; }

        /// <summary>
        /// Third feeding time
        /// </summary>
        public DateTime? Time4 { get; set; }

        /// <summary>
        /// Fifth feeding time
        /// </summary>
        public DateTime? Time5 { get; set; }
    }
}
