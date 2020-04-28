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
using NUnit.Framework.Internal;

namespace Microwave.Integration.Test
{
    [TestFixture]
    class Step7_new
    {
        //Real modules
        private Display _display;
        private Output _output;
        private CookController _cookController;

        //Faked modules
        private IPowerTube _powerTube;
        private ITimer _timer;
        private IUserInterface _userInterface;

        private StringWriter _stringWriter;

        [SetUp]
        public void Setup()
        {
            _userInterface = Substitute.For<IUserInterface>();
            _timer = Substitute.For<ITimer>();
            _powerTube = Substitute.For<IPowerTube>();

            _output = new Output();
            _display = new Display(_output);
            _cookController = new CookController(_timer, _display, _powerTube);

            _stringWriter = new StringWriter();
        }

        [Test]
        public void OnTimerTick_DisplayUpdated()
        {
            //Use NSubstitute to make the time remaining return 74 seconds
            int time = 74;
            _timer.TimeRemaining.Returns(time);

            int min = time / 60;
            int sec = time % 60;

            Console.SetOut(_stringWriter);
            //Raise the timer tick event, triggering the OnTimerTick event handler in CookController
            _timer.TimerTick += Raise.EventWith(new object(), new EventArgs());

            Assert.That(_stringWriter.ToString(), Contains.Substring($"Display shows: {min:D2}:{sec:D2}"));
        }
    }
}
