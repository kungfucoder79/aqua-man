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
    public class FeedingController : Controller
    {
        private IFormDataService _formDataService;

        /// <summary>
        /// Constructs a new 
        /// </summary>
        /// <param name="formDataService"></param>
        public FeedingController(IFormDataService formDataService)
        {
            _formDataService = formDataService;
        }
        // GET: /<controller>/
        /// <summary>
        /// Gets the <see cref="FeedingTimes"/> model for the tank view
        /// </summary>
        /// <returns></returns>
        public IActionResult FeedingTimes()
        {
            FeedingTimes feedingTimes = _formDataService.GetFeedingTimes();
            return View(feedingTimes);
        }
        // GET: /<controller>/
        /// <summary>
        /// Posts the <see cref="FeedingTimes"/> from the users, and saves it.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult FeedingTimes(FeedingTimes feedingTimes)
        {
            if (ModelState.IsValid)
            {
                _formDataService.SetFeedingTimes(feedingTimes);
                return RedirectToAction("Index", nameof(HomeController).RemoveControllerFromName());
            }
            return View(feedingTimes);
        }
}
}