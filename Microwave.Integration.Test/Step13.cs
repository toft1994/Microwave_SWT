using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NUnit.Framework;


namespace Microwave.Integration.Test
{
    public class Step13
    {
        private IOutput _output;
        private IDoor _uut;
        private StringWriter consoleOut;
        private ILight _light;
        private IUserInterface _userInterface;
        private IDisplay _display;
        private ICookController _cookController;
        private IPowerTube _powerTube;
        private ITimer _timer;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDoor _door;

        [SetUp]
        public void SetUp()
        {
            _uut = new Door();
            _output = new Output();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _light = new Light(_output);
            consoleOut = new StringWriter();
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _uut, _display, _light,
                _cookController);
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
            //uut = new Display(output);
        }

        [Test]
        public void Check_DoorOpen_LightOn()
        {
            Console.SetOut(consoleOut);
            _uut.Open();
            Assert.That(consoleOut.ToString().Equals("Light is turned on\r\n"));
        }

        [Test]
        public void Check_DoorClose_LightOff()
        {
            _uut.Open();
            Console.SetOut(consoleOut);
            _uut.Close();
            Assert.That(consoleOut.ToString().Contains("Light is turned off\r\n"));
        }
    }
}