using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Microwave.Application;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Integration.Test
{
    [TestFixture]
    public class Step8
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

        // Test Display output when pressing power button multiples of times 
        // make sure to include edge cases plus some random cases
        [TestCase(1, "Display shows: 50 W\r\n")]
        [TestCase(2, "Display shows: 100 W\r\n")]
        [TestCase(4, "Display shows: 200 W\r\n")]
        [TestCase(8, "Display shows: 400 W\r\n")]
        [TestCase(13, "Display shows: 650 W\r\n")]
        [TestCase(14, "Display shows: 700 W\r\n")]
        [TestCase(15, "Display shows: 50 W\r\n")]
        [TestCase(27, "Display shows: 650 W\r\n")]
        [TestCase(29, "Display shows: 50 W\r\n")]
        public void OnPowerPressed_Ready_ShowsExpectedPower(int presses, string response)
        {
            // Check input
            if (presses <= 0)
            {
                throw new ArgumentException("presses must be a positive integer");
            }

            if (String.IsNullOrEmpty(response))
            {
                throw new ArgumentException("response must not be null or empty");
            }

            // Press presses times -1
            for (int i = 0; i < presses - 1; i++)
            {
                // Press button once
                _powerButton.Pressed += Raise.Event();
            }

            // Capture output
            Console.SetOut(_stringWriter);

            // Press button one last time
            _powerButton.Pressed += Raise.Event();

            // Check if light turned off
            Assert.That(_stringWriter.ToString(), Is.EqualTo(response));
        }

        // Test multiples of presses of time button
        // some random samples have been included. Edge cases have
        // been added as limit to time has been added (max 60 minutes)
        [TestCase(1, "Display shows: 01:00\r\n")]
        [TestCase(2, "Display shows: 02:00\r\n")]
        [TestCase(30, "Display shows: 30:00\r\n")]
        [TestCase(45, "Display shows: 45:00\r\n")]
        [TestCase(60, "Display shows: 60:00\r\n")]
        [TestCase(61, "Display shows: 01:00\r\n")]
        [TestCase(120, "Display shows: 60:00\r\n")]
        public void OnTimePressed_Ready_ShowsExpectedTime(int presses, string response)
        {
            // Check input
            if (presses <= 0)
            {
                throw new ArgumentException("presses must be a positive integer");
            }

            if (String.IsNullOrEmpty(response))
            {
                throw new ArgumentException("response must not be null or empty");
            }

            // Press power once
            _powerButton.Pressed += Raise.Event();

            // Press presses times -1
            for (int i = 0; i < presses - 1; i++)
            {
                // Press button once
                _timeButton.Pressed += Raise.Event();
            }

            // Capture output
            Console.SetOut(_stringWriter);

            // Press button one last time
            _timeButton.Pressed += Raise.Event();

            // Check if light turned off
            Assert.That(_stringWriter.ToString(), Is.EqualTo(response));
        }

        [Test]
        public void OnCookingIsDone_Cooking_DisplayIsCleared()
        {
            // Simulate cooking
            _powerButton.Pressed += Raise.Event();
            _timeButton.Pressed += Raise.Event();
            _startCancelButton.Pressed += Raise.Event();

            // State should be cooking for 1 second at 50W

            // Capture output
            Console.SetOut(_stringWriter);

            // Wait for at least 1 second
            Thread.Sleep(1500);

            // Check if display was cleared after cooking done
            Assert.That(_stringWriter.ToString(), Contains.Substring("Display cleared"));
        }

        [Test]
        public void OnDoorOpened_Cooking_DisplayIsCleared()
        {
            // Simulate cooking
            _powerButton.Pressed += Raise.Event();
            _timeButton.Pressed += Raise.Event();
            _startCancelButton.Pressed += Raise.Event();

            // State should be cooking for 1 second at 50W

            // Capture output
            Console.SetOut(_stringWriter);

            // Open door while cooking
            _door.Opened += Raise.Event();

            // Check if display was cleared after cooking done
            Assert.That(_stringWriter.ToString(), Contains.Substring("Display cleared"));
        }

        [Test]
        public void OnStartCancelPressed_Cooking_DisplayIsCleared()
        {
            // Simulate cooking
            _powerButton.Pressed += Raise.Event();
            _timeButton.Pressed += Raise.Event();
            _startCancelButton.Pressed += Raise.Event();

            // State should be cooking for 1 second at 50W

            // Capture output
            Console.SetOut(_stringWriter);

            // Open door while cooking
            _startCancelButton.Pressed += Raise.Event();

            // Check if display was cleared after cooking done
            Assert.That(_stringWriter.ToString(), Contains.Substring("Display cleared"));
        }
    }
}
