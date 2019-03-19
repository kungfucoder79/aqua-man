using Aqua_Control;
using AquaMan.Extensions;
using AquaMan.Models;
using AquaMan.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Controllers
{
    /// <summary>
    /// Controller for the tank view
    /// </summary>
    public class LayoutController : Controller
    {
        // GET: /<controller>/
        /// <summary>
        /// Gets the Current Time
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCurrentTime()
        {
            return Ok(DateTime.Now.ToLongTimeString());
        }
    }
}