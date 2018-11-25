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
    public class MaintainaceController : Controller
    {
        private IAquaPinController _aquaPinController;
        private IAquaI2CController _aquaI2CController;
        private IPinMasterController _pinMasterController;

        private double _waterHeight = 0.0;

        public MaintainaceController(IAquaPinController aquaPinController, IAquaI2CController aquaI2CController, IPinMasterController pinMasterController)
        {
            _aquaPinController = aquaPinController;
            _aquaI2CController = aquaI2CController;
            _pinMasterController = pinMasterController;
        }

        public IActionResult Maintainace()
        {
            return View();
        }

        public IActionResult WaterChange()
        {
            _pinMasterController.WaterChange();
            return Ok();
        }

        public IActionResult TopLevel()
        {
            _aquaI2CController.SetTopLevel();
            return Ok();
        }
        public IActionResult Fill()
        {
            if (!_aquaPinController.IsFillActive && !_aquaPinController.IsDrainActive && !_aquaPinController.IsPumpActive)
                _aquaPinController.FillSaltWater();
            return Ok();
        }

        public IActionResult Drain()
        {
            if (!_aquaPinController.IsFillActive && !_aquaPinController.IsDrainActive && !_aquaPinController.IsPumpActive)
                _aquaPinController.Drain();
            return Ok();
        }

        public IActionResult V1()
        {
            bool val = _aquaPinController.ToggleValve1();
            return Ok(val);
        }

        public IActionResult V2()
        {
            bool val = _aquaPinController.ToggleValve2();
            return Ok(val);
        }

        public IActionResult V3()
        {
            bool val = _aquaPinController.ToggleValve3();
            return Ok(val);
        }

        public IActionResult V4()
        {
            bool val = _aquaPinController.ToggleValve4();
            return Ok(val);
        }

        public IActionResult V5()
        {
            bool val = _aquaPinController.ToggleValve5();
            return Ok(val);
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
