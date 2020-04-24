using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Microwave.Application;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;

namespace Microwave.Integration.Test
{
    [TestFixture]
    public class Step7
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
            _top = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);

            // Assign _top as _cookController UI
            _cookController.UI = _top;
        }
    }
}
