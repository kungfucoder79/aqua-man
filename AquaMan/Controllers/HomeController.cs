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
        private IPinMasterController _pinMasterController;

        private double _waterHeight = 0.0;

        public HomeController(IAquaPinController aquaPinController, IAquaI2CController aquaI2CController, IPinMasterController pinMasterController)
        {
            _aquaPinController = aquaPinController;
            _aquaI2CController = aquaI2CController;
            _pinMasterController = pinMasterController;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TopLevel()
        {
            _aquaI2CController.SetTopLevel();
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
