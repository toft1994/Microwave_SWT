using System;
using System.Collections.Generic;
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
    public class Step10
    {
        private IOutput _output;
        private Light _light;
        private UserInterface _userInterface;
        private Display _display;
        private CookController _uut;
        private PowerTube _powerTube;
        private Timer _timer;
        private IButton _buttonPower;
        private IButton _buttonTime; 
        private IButton _buttonStartCancel;
        private IDoor _door;

        [SetUp]
        public void SetUp()
        {
            // Fakes
            _buttonPower = Substitute.For<IButton>();
            _buttonTime = Substitute.For<IButton>();
            _buttonStartCancel = Substitute.For<IButton>();
            _output = Substitute.For<IOutput>();

            // Real Modules
            _timer = new Timer();
            _door = new Door();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _light = new Light(_output);
            _uut = new CookController(_timer, _display, _powerTube, _userInterface);
            _userInterface = new UserInterface(_buttonPower, _buttonTime, _buttonStartCancel, _door, _display, _light,
                _uut);
            _uut.UI = _userInterface;
        }

        [Test]
        public void CookingIsDone_LightIsTurnedOff_InfoSentToConsole()
        {
            // To stimulate CookingIsDone() we need to set the UI state to COOKING.
            // Therefore we need to stimulate the events from the buttons! 
            _buttonPower.Pressed += Raise.Event();
            _buttonTime.Pressed += Raise.Event();
            _buttonStartCancel.Pressed += Raise.Event();

            // Time is multiplied by 60 because its the lowest time
            Thread.Sleep(1100*60);

            _output.Received(1).OutputLine("Light is turned off");
        }

        [Test]
        public void CookingIsDone_DisplayCleared_InfoSentToConsole()
        {
            // To stimulate CookingIsDone() we need to set the UI state to COOKING.
            // Therefore we need to stimulate the events from the buttons!
            _buttonPower.Pressed += Raise.Event();
            _buttonTime.Pressed += Raise.Event();
            _buttonStartCancel.Pressed += Raise.Event();
            // Time is multiplied by 60 because its the lowest time
            Thread.Sleep(1100 * 60);

            _output.Received(1).OutputLine("Display cleared");
        }
    }
}
