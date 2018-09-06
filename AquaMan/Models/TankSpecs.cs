using System;
using System.Collections.Generic;
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
        public double Height { get; set; }

        /// <summary>
        /// Width of the tank
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Depth of the tank
        /// </summary>
        public DateTime Depth { get; set; }
    }
}
