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
    public class Step2
    {
        private IOutput output;
        private IPowerTube uut;
        private StringWriter consoleOut;

        [SetUp]
        public void SetUp()
        {
            consoleOut = new StringWriter();
            output = new Output();
            uut = new PowerTube(output);
        }
        
        [TestCase(100)]
        [TestCase(1)]
        public void TurnOn_powerOK_MessageSentToConsole(int power)
        {
            Console.SetOut(consoleOut);
            uut.TurnOn(power);
            Assert.That(consoleOut.ToString().Equals($"PowerTube works with {power}\r\n"));
        }

        [TestCase(101)]
        [TestCase(0)]
        public void TurnOn_powerNotOK_ExceptionThrown(int power)
        {
            Console.SetOut(consoleOut);
            Assert.That(() => uut.TurnOn(power), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void TurnOff_powerOff_MessageSentToConsole()
        {
            uut.TurnOn(50);
            Console.SetOut(consoleOut);
            uut.TurnOff();
            Assert.That(consoleOut.ToString().Equals("PowerTube turned off\r\n"));
        }
    }
}
