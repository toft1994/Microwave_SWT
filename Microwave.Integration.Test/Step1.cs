using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NUnit.Framework;

namespace Microwave.Integration.Test
{
    [TestFixture]
    public class Step1
    {
        private IOutput output;
        private ILight uut;
        private StringWriter consoleOut;

        [SetUp]
        public void SetUp()
        {
            consoleOut = new StringWriter();
            output = new Output();
            uut = new Light(output);
        }


        [Test]
        public void TurnOn_lightTurnsOn_MessageSentToConsole()
        {
            Console.SetOut(consoleOut);
            uut.TurnOn();
            Assert.That(consoleOut.ToString().Equals("Light is turned on\r\n"));
        }

        [Test]
        public void TurnOff_lightTurnsOff_MessageSentToConsole()
        {
            uut.TurnOn();
            Console.SetOut(consoleOut);
            uut.TurnOff();
            Assert.That(consoleOut.ToString().Equals("Light is turned off\r\n"));
        }
    }
}
