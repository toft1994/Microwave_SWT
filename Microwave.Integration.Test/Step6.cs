﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Integration.Test
{
    [TestFixture]
    class Step6
    {
        //All the necessary (real) modules
        private CookController _cookController;
        private PowerTube _powerTube;
        private Timer _timer;
        private Display _display;

        //Fake modules necessary for initializing a CookController
        private IUserInterface _userInterface;
        private IOutput _output;



        [SetUp]
        public void Setup()
        {
            //Create fakes
            _userInterface = Substitute.For<IUserInterface>();
            _output = Substitute.For<IOutput>();

            //Initialize the real module normally
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);

            //Initialize top module (CookController)
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
        }

        [Test]
        public void StartCooking_PowerTubeTurnsOn()
        {
            int power = 10;
            int time = 2;

            _cookController.StartCooking(power, time);

            _output.Received(1).OutputLine($"PowerTube works with {power}");
        }

        [Test]
        public void StartCooking_PowerTubeTurnsOff()
        {
            int power = 10;
            int time = 2;

            //Call startCooking with the designated time
            _cookController.StartCooking(power, time);

            //Wait until that amount of time has passed (converted to milliseconds)
            Thread.Sleep(time*1500);

            _output.Received(1).OutputLine("PowerTube turned off");
        }

        [Test]
        public void StartCooking_OutputCorrectTime()
        {
            int power = 10;
            int seconds = 2;
            _cookController.StartCooking(power, seconds);
            
            //Wait one second, so the microwave is still on
            Thread.Sleep(seconds * 1500);

            for (int i = seconds-1; i >= 0; i--)
            {
                // Check if all seconds is displayed. 
                int min = i / 60;
                int sec = i % 60;
                
                _output.Received(1).OutputLine($"Display shows: {min:D2}:{sec:D2}");
            }
        }

        [Test]
        public void StopCooking_WhileRunning_PowerTubeStops()
        {
            int power = 10;
            int time = 2;
            _cookController.StartCooking(power, time);

            _cookController.Stop();
            _output.Received(1).OutputLine("PowerTube turned off");
        }

        [Test]
        public void StopCooking_WhileNotRunning_NothingHappens()
        {
            //Make sure powertube is already stopped
            _cookController.Stop();
            _cookController.Stop();
            _output.Received(0).OutputLine("");
        }
    }
}
