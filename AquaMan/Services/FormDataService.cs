using AquaMan.Models;
using System;
using System.Collections.Generic;

namespace AquaMan.Services
{
    public class FormDataService : IFormDataService
    {
        public IEnumerable<DateTime> GetDateTimes()
        {
            throw new NotImplementedException();
        }

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

        public void SetDateTimes(IEnumerable<DateTime> dateTimes)
        {
            throw new NotImplementedException();
        }

        public void SetTankSpecs(TankSpecs tankSpecs)
        {
            //save stuff
        }
    }
}
