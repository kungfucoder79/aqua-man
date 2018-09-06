using Microsoft.AspNetCore.Mvc.Rendering;
using AquaMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Services
{
    public interface IFormDataService
    {
        IEnumerable<DateTime> GetDateTimes();
        void SetDateTimes(IEnumerable<DateTime> dateTimes);
        void SetTankSpecs(TankSpecs tankSpecs);
        TankSpecs GetTankSpecs();
    }
}
