using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Integration.Test
{
    [TestFixture]
    class Step4
    {
        //All the necessary (real) modules
        private CookController _cookController;
        private Display _display;
        private PowerTube _powerTube;
        private Output _output;

        //Fake modules necessary for initializing a CookController
        private IUserInterface _userInterface;
        private ITimer _timer;

        //Stringwriter used for capturing output
        private StringWriter _stringWriter;

        [SetUp]
        public void Setup()
        {
            //Initialize the real modules normally
            _output = new Output();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);

            //Create fakes
            _userInterface = Substitute.For<IUserInterface>();
            _timer = Substitute.For<ITimer>();

            //Initialize top module (CookController)
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);

            _stringWriter = new StringWriter();
        }

        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        public void StartCooking_PowerTubeOn(int power)
        {
            //Time parameter isn't important for this test
            int time = 10;

            //Capture console output with the stringwriter
            Console.SetOut(_stringWriter);

            //Call the StartCooking method
            _cookController.StartCooking(power, time);

            Assert.That(_stringWriter.ToString(), Contains.Substring($"PowerTube works with {power}"));
        }

        [Test]
        public void Stop_PoweredOn_PowerTubeOff()
        {
            //Call start method to make sure powertube is on initially
            _cookController.StartCooking(10,10);

            Console.SetOut(_stringWriter);

            _cookController.Stop();

            Assert.That(_stringWriter.ToString(), Contains.Substring($"PowerTube turned off"));
        }

        [Test]
        public void Stop_PoweredOff_NothingHappens()
        {
            //Call stop method to ensure power tube is already off
            _cookController.Stop();

            Console.SetOut(_stringWriter);

            //Call stop while powertube is already off
            _cookController.Stop();

            //Assert that nothing was written to the stringwriter
            Assert.That(_stringWriter.ToString(), Is.Empty);
        }
    }
}
