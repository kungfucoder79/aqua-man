using Aqua_Control;
using AquaMan.Extensions;
using AquaMan.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AquaMan.Controllers
{
    public class HomeController : Controller
    {
        private IAquaPinController _aquaPinController;
        private IAquaI2CController _aquaI2CController;
        private double _waterHeight = 0.0;

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
            if (!_aquaPinController.IsPumpActive)
                _aquaPinController.Fill();
            return Ok();
        }

        public IActionResult Drain()
        {
            if (!_aquaPinController.IsPumpActive)
                _aquaPinController.Drain();
            return Ok();
        }

        public IActionResult V1()
        {
            _aquaPinController.ToggleValve1();
            return Ok();
        }

        public IActionResult V2()
        {
            _aquaPinController.ToggleValve2();
            return Ok();
        }

        public IActionResult V3()
        {
            _aquaPinController.ToggleValve3();
            return Ok();
        }

        public IActionResult V4()
        {
            _aquaPinController.ToggleValve4();
            return Ok();
        }

        public IActionResult V5()
        {
            _aquaPinController.ToggleValve5();
            return Ok();
        }

        public IActionResult Feed()
        {
            _aquaPinController.Feed();
            return Ok();
        }
        public IActionResult ResetI2C()
        {
            _aquaI2CController.Reset();
            return Ok();
        }

        public IActionResult Stop()
        {
            _aquaPinController.Stop();
            return Ok();
        }

        public IActionResult Calabrate()
        {
            _aquaI2CController.CalabrateSensor();
            return Ok();

        }
        public IActionResult GetWaterHeight()
        {
            _waterHeight = _aquaI2CController.WaterHeight;
            return Ok(_waterHeight);
        }

    }
}
