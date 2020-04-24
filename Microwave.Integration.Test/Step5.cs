using System;
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
    class Step5
    {
        //All the necessary (real) modules
        private CookController _cookController;
        private PowerTube _powerTube;
        private Timer _timer;
        private Output _output;
        private Display _display;

        //Fake modules necessary for initializing a CookController
        private IUserInterface _userInterface;

        //Stringwriter used for capturing output
        private StringWriter _stringWriter;

        [SetUp]
        public void Setup()
        {
            //Initialize the real module normally
            _timer = new Timer();
            _output = new Output();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            //Create fakes
            _userInterface = Substitute.For<IUserInterface>();


            //Initialize top module (CookController)
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);

            _stringWriter = new StringWriter();
        }

        [Test]
        public void StartCooking_PowerTubeTurnsOn()
        {
            int power = 10;
            int time = 2;

            Console.SetOut(_stringWriter);
            _cookController.StartCooking(power, time);

            Assert.That(_stringWriter.ToString(), Contains.Substring($"PowerTube works with {power}"));
        }

        [Test]
        public void StartCooking_PowerTubeTurnsOff()
        {
            int power = 10;
            int time = 2;

            //Call startCooking with the designated time
            _cookController.StartCooking(power, time);
            Console.SetOut(_stringWriter);

            //Wait until that amount of time has passed (converted to milliseconds)
            Thread.Sleep(time*1000);

            Assert.That(_stringWriter.ToString(), Contains.Substring($"PowerTube turned off"));
        }

        [Test]
        public void StartCooking_OutputCorrectTime()
        {
            int power = 10;
            int time = 2;
            _cookController.StartCooking(power, time);
            Console.SetOut(_stringWriter);

            //If the tick works, there will be a message displaying the initial time
            int min = time / 60;
            int sec = time % 60;

            //Wait one second, so the microwave is still on
            Thread.Sleep(time * 1000);

            Assert.That(_stringWriter.ToString(), Contains.Substring($"Display shows: {min:D2}:{sec:D2}"));
        }

        [Test]
        public void StopCooking_WhileRunning_PowerTubeStops()
        {
            int power = 10;
            int time = 2;
            _cookController.StartCooking(power, time);
            Console.SetOut(_stringWriter);
            _cookController.Stop();
            Assert.That(_stringWriter.ToString(), Contains.Substring($"PowerTube turned off"));
        }

        [Test]
        public void StopCooking_WhileNotRunning_NothingHappens()
        {
            //Make sure powertube is already stopped
            _cookController.Stop();
            Console.SetOut(_stringWriter);
            _cookController.Stop();
            Assert.That(_stringWriter.ToString(), Is.Empty);
        }
    }
}
