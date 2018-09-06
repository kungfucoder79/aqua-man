using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveControllerFromName(this string fullControllerName)
        {
            return fullControllerName.Replace("Controller", "");
        }
    }
}
