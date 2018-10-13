using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AquaMan.Models;
using Aqua_Control;
using AquaMan.Extensions;
using System.Net;

namespace AquaMan.Controllers
{
    public class HomeController : Controller
    {
        private IAquaPinController _aquaPinController;
        private IAquaI2CController _aquaI2CController;

        public HomeController(IAquaPinController aquaPinController, IAquaI2CController aquaI2CController)
        {
            _aquaPinController = aquaPinController;
            _aquaI2CController = aquaI2CController;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Fill()
        {
            _aquaPinController.Fill();
            return Ok();
        }

        public IActionResult Drain()
        {
            _aquaPinController.Drain();
            return Ok();
        }

        public IActionResult Feed()
        {
            _aquaPinController.FeedMe(2000);
            return Ok();
        }
        public IActionResult ResetI2C()
        {
            _aquaI2CController.Reset();
            return Ok();
        }
        
    }
}
