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
    class Step6
    {
        private CookController _cookController;
        private PowerTube _powerTube;
        private Timer _timer;
        private Output _output;
        private Display _display;

        private IUserInterface _userInterface;

        private StringWriter _stringWriter;

        [SetUp]
        public void Setup()
        {
            //Initialize modules
            _timer = new Timer();
            _output = new Output();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _userInterface = Substitute.For<IUserInterface>();
            _cookController = new CookController(_timer, _display, _powerTube, _userInterface);

            _stringWriter = new StringWriter();
        }

        [Test]
        public void Start_OutputRemainingTime()
        {
            Console.SetOut(_stringWriter);
            int time = 2;
            _timer.Start(time);

            int min = time / 60;
            int sec = time % 60;
            Thread.Sleep(time*1000);
            Assert.That(_stringWriter.ToString(), Contains.Substring($"Display shows: {min:D2}:{sec:D2}"));
        }
    }
}