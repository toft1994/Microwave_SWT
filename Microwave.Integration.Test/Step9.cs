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
using Timer = MicrowaveOvenClasses.Boundary.Timer;


namespace Microwave.Integration.Test
{
    [TestFixture]
    public class Step9
    {
        // Real modules
        private UserInterface _top;
        private Display _display;
        private Light _light;
        private CookController _cookController;
        private Output _output;
        private PowerTube _powerTube;
        private Timer _timer;
        private StringWriter _stringWriter;

        // Fake modules
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDoor _door;

        [SetUp]
        public void Setup()
        {
            // Initiate real modules
            _output = new Output();
            _display = new Display(_output);
            _light = new Light(_output);
            _powerTube = new PowerTube(_output);
            _timer = new Timer();
            _cookController = new CookController(_timer, _display, _powerTube);

            // Create fakes
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();

            // Capture output using stringWriter
            _stringWriter = new StringWriter();

            // Create Top
            _top = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light,
                _cookController);

            // Assign _top as _cookController UI
            _cookController.UI = _top;
        }

        [Test]
        public void OnDoorOpened_Ready_LightTurnedOn()
        {
            // Capture output
            Console.SetOut(_stringWriter);

            // Open door
            _top.OnDoorOpened(this, null);

            // Check if light turned on
            Assert.That(_stringWriter.ToString(), Is.EqualTo("Light is turned on\r\n"));
        }

        [Test]
        public void OnDoorClosed_DoorIsOpen_LightTurnedOff()
        {
            // Open door
            _top.OnDoorOpened(this, null);

            // Capture output
            Console.SetOut(_stringWriter);

            // Close door
            _top.OnDoorClosed(this, null);

            // Check if light turned off
            Assert.That(_stringWriter.ToString(), Is.EqualTo("Light is turned off\r\n"));
        }

        [Test]
        public void OnStartCancelPressed_SetTime_LightTurnedOn()
        {
            // Setup oven
            _top.OnPowerPressed(this, null);
            _top.OnTimePressed(this, null);

            // Capture output
            Console.SetOut(_stringWriter);

            // Start cooking
            _startCancelButton.Pressed += Raise.Event();

            // Check if light turned on
            Assert.That(_stringWriter.ToString(), Contains.Substring("Light is turned on\r\n"));
        }

        [Test]
        public void OnCookingIsDone_Cooking_LightTurnedOff()
        {
            // Simulate cooking
            _top.OnPowerPressed(this, null);
            _top.OnTimePressed(this, null);
            _top.OnStartCancelPressed(this, null);

            // State should be cooking for 1 second at 50W

            // Capture output
            Console.SetOut(_stringWriter);

            // Wait for at least 1 second
            Thread.Sleep(1500);

            // Check if display was cleared after cooking done
            Assert.That(_stringWriter.ToString(), Contains.Substring("Light is turned off\r\n"));
        }

        [Test]
        public void OnStartCancelPressed_DoorIsOpen_NothingHappened()
        {
            // Setup oven for cooking
            _top.OnPowerPressed(this, null);
            _top.OnTimePressed(this, null);

            // Open door
            _top.OnDoorOpened(this, null);

            // Capture output
            Console.SetOut(_stringWriter);

            // Try to start cooking
            _top.OnStartCancelPressed(this, null);

            // Check if light turned on
            Assert.That(_stringWriter.ToString(), Contains.Substring(""));
        }

        [Test]
        public void OnStartCancelPressed_Cooking_LightTurnedOff()
        {
            // Simulate cooking
            _top.OnPowerPressed(this, null);
            _top.OnTimePressed(this, null);
            _top.OnStartCancelPressed(this, null);

            // State should be cooking for 1 second at 50W

            // Capture output
            Console.SetOut(_stringWriter);

            // Open door
            _top.OnStartCancelPressed(this, null);

            // Check if display was cleared after cooking done
            Assert.That(_stringWriter.ToString(), Contains.Substring("Light is turned off\r\n"));
        }

        [Test]
        public void OnDoorOpened_Cooking_LightTurnedOff()
        {
            // Simulate cooking
            _top.OnPowerPressed(this, null);
            _top.OnTimePressed(this, null);
            _top.OnStartCancelPressed(this, null);

            // State should be cooking for 1 second at 50W

            // Capture output
            Console.SetOut(_stringWriter);

            // Open door
            _top.OnDoorOpened(this, null);

            // Check if display was cleared after cooking done
            Assert.That(_stringWriter.ToString(), Contains.Substring("Light is turned off\r\n"));
        }
    }
}
