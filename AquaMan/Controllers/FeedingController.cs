using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqua_Control;
using AquaMan.Extensions;
using AquaMan.Models;
using AquaMan.Services;
using Microsoft.AspNetCore.Mvc;

namespace AquaMan.Controllers
{
    /// <summary>
    /// Controller for the Feeding view
    /// </summary>
    public class FeedingController : Controller
    {
        private IFormDataService _formDataService;
        IAquaPinController _aquaPinController;

        /// <summary>
        /// Constructs a new <see cref="FeedingController"/>
        /// </summary>
        /// <param name="formDataService"></param>
        public FeedingController(IFormDataService formDataService, IAquaPinController aquaPinController)
        {
            _formDataService = formDataService;
            _aquaPinController = aquaPinController;
        }
        // GET: /<controller>/
        /// <summary>
        /// Gets the <see cref="FeedingTimes"/> model for the feeding view
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
                _aquaPinController.UpdateFeedingTimes(feedingTimes.Feedings,feedingTimes.Pinches);
                return RedirectToAction("Index", nameof(HomeController).RemoveControllerFromName());
            }
            return View(feedingTimes);
        }
}
}