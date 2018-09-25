using System;
using System.Collections.Generic;
using System.Text;

namespace Aqua_Control
{ 
    /// <summary>
    /// Empty impelmentation for testing on a device with out pins
    /// </summary>
    public class PinControllerEmpty : IAquaPinController
    {
        /// <summary>
        /// Empty Drain
        /// </summary>
        public void Drain()
        {
            
        }

        /// <summary>
        /// Empty fill
        /// </summary>
        public void Fill()
        {

        }
    }
}
