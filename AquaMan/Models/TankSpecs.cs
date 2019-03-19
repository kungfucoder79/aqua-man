using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Models
{
    /// <summary>
    /// Class for the tank specifications
    /// </summary>
    public class TankSpecs
    {
        /// <summary>
        /// Time to initiate the water change sequence
        /// </summary>
        [Required(ErrorMessage = "* A time must be specified for water changes")]
        public DateTime WaterChangeTime { get; set; }
        /// <summary>
        /// Height of the tank
        /// </summary>
        [Required]
        [RegularExpression(@"\d+(\.\d{1,4})?$", ErrorMessage = "* Must be at least a four digit decimal (1.0000)")]
        public double Height { get; set; }

        /// <summary>
        /// Width of the tank
        /// </summary>
        [Required]
        [RegularExpression(@"\d+(\.\d{1,4})?$", ErrorMessage = "* Must be at least a four digit decimal (1.0000)")]
        public double Width { get; set; }

        /// <summary>
        /// Depth of the tank
        /// </summary>
        [Required]
        [RegularExpression(@"\d+(\.\d{1,4})?$", ErrorMessage = "* Must be at least a four digit decimal (1.0000)")]
        public double Depth { get; set; }
    }
}
