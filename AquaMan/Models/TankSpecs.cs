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
        /// Height of the tank
        /// </summary>
        [Required]
        [RegularExpression(@"\d+(\.\d{1,2})?$", ErrorMessage = "* Must be a two digit decimal (1.00)")]
        public double Height { get; set; }

        /// <summary>
        /// Width of the tank
        /// </summary>
        [Required]
        [RegularExpression(@"\d+(\.\d{1,2})?$", ErrorMessage = "* Must be a two digit decimal (1.00)")]
        public double Width { get; set; }

        /// <summary>
        /// Depth of the tank
        /// </summary>
        [Required]
        [RegularExpression(@"\d+(\.\d{1,2})?$", ErrorMessage = "* Must be a two digit decimal (1.00)")]
        public double Depth { get; set; }
    }
}
