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
    public class Step12
    {

        private IOutput _output;
        private ILight _light;
        private IUserInterface _userInterface;
        private IDisplay _display;
        private ICookController _cookController;
        private IPowerTube _powerTube;
        private ITimer _timer;
        private IButton _uutPower; //Power
        private IButton _uutTime; //Time
        private IButton _uutStartCancel; //StartCancel
        private IDoor _door;

        [SetUp]
        public void SetUp()
        {
            _uutPower = new Button();
            _uutTime = new Button();
            _uutStartCancel = new Button();
            _output = Substitute.For<IOutput>();

            _timer = new Timer();
            _door = new Door();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _light = new Light(_output);
            _userInterface = new UserInterface(_uutPower, _uutTime, _uutStartCancel, _door, _display, _light,
                _cookController);
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);
        }

        [Test]
        public void PowerButton_Pressed()
        {
            _uutPower.Press();
            _output.Received(1).OutputLine("Display shows: 50 W");
        }

        [Test]
        public void PowerButton_Pressed2()
        {
            _uutPower.Press();
            _uutPower.Press();
            _output.Received(1).OutputLine("Display shows: 100 W");
        }

        [Test]
        public void TimeButton_Pressed()
        {
            var min = 1;
            var sec = 0;

            _uutPower.Press();
            _uutTime.Press();
            _output.Received(1).OutputLine($"Display shows: {min:D2}:{sec:D2}");
        }

        [Test]
        public void StartCancelButton_DoorClosed_PressedStart()
        {
            _uutPower.Press();
            _uutStartCancel.Press();
            
            _output.Received(1).OutputLine("Display cleared");
        }
    }
}