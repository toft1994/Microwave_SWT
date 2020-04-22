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
    public class Step3
    {
        private IOutput output;
        private IDisplay uut;
        private StringWriter consoleOut;

        [SetUp]
        public void SetUp()
        {
            consoleOut = new StringWriter();
            output = new Output();
            uut = new Display(output);
        }


        [Test]
        public void Clear_Cleared_MessageSentToConsole()
        {
            Console.SetOut(consoleOut);
            uut.Clear();
            Assert.That(consoleOut.ToString().Equals("Display cleared\r\n"));
        }

        [TestCase(100)]
        [TestCase(50)]
        [TestCase(1)]
        public void ShowPower_powerIsShown_MessageSentToConsole(int power)
        {
            Console.SetOut(consoleOut);
            uut.ShowPower(power);
            Assert.That(consoleOut.ToString().Equals($"Display shows: {power} W\r\n"));
        }

        [TestCase(100, 60)]
        [TestCase(50, 30)]
        [TestCase(0, 0)]
        public void ShowTime_timeIsShown_MessageSentConsole(int minutes, int seconds)
        {
            Console.SetOut(consoleOut);
            uut.ShowTime(minutes, seconds);
            Assert.That(consoleOut.ToString().Equals($"Display shows: {minutes:D2}:{seconds:D2}\r\n"));
        }
    }
}
