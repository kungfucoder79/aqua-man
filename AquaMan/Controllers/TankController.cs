using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquaMan.Extensions;
using AquaMan.Models;
using AquaMan.Services;
using Microsoft.AspNetCore.Mvc;

namespace AquaMan.Controllers
{
    /// <summary>
    /// Controller for the tank view
    /// </summary>
    public class TankController : Controller
    {
        private IFormDataService _formDataService;

        /// <summary>
        /// Constructs a new 
        /// </summary>
        /// <param name="formDataService"></param>
        public TankController(IFormDataService formDataService)
        {
            _formDataService = formDataService;
        }
        // GET: /<controller>/
        /// <summary>
        /// Gets the <see cref="TankSpecs"/> model for the tank view
        /// </summary>
        /// <returns></returns>
        public IActionResult TankSpecs()
        {
            TankSpecs tankSpecs = _formDataService.GetTankSpecs();
            return View(tankSpecs);
        }
        // GET: /<controller>/
        /// <summary>
        /// Posts the <see cref="TankSpecs"/> from the users, and saves it.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TankSpecs(TankSpecs tankSpecs)
        {
            if (ModelState.IsValid)
            {
                _formDataService.SetTankSpecs(tankSpecs);
                return RedirectToAction("Index", nameof(HomeController).RemoveControllerFromName());
            }
            
            return View(tankSpecs);
        }
}
}