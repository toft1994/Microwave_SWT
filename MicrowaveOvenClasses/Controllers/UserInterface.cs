﻿using System;
using System.Runtime.Serialization;
using MicrowaveOvenClasses.Interfaces;

namespace MicrowaveOvenClasses.Controllers
{
    public class UserInterface : IUserInterface
    {
        private enum States
        {
            READY, SETPOWER, SETTIME, COOKING, DOOROPEN
        }

        private States myState = States.READY;

        private ICookController myCooker;
        private ILight myLight;
        private IDisplay myDisplay;

        private uint powerLevel = 50;
        private uint time = 1;

        public UserInterface(
            IButton powerButton,
            IButton timeButton,
            IButton startCancelButton,
            IDoor door,
            IDisplay display,
            ILight light,
            ICookController cooker)
        {
            powerButton.Pressed += new EventHandler(OnPowerPressed);
            timeButton.Pressed += new EventHandler(OnTimePressed);
            startCancelButton.Pressed += new EventHandler(OnStartCancelPressed);

            door.Closed += new EventHandler(OnDoorClosed);
            door.Opened += new EventHandler(OnDoorOpened);

            myCooker = cooker;
            myLight = light;
            myDisplay = display;
        }

        public void OnPowerPressed(object sender, EventArgs e)
        {
            switch (myState)
            {
                case States.READY:
                    myDisplay.ShowPower((int)powerLevel);
                    myState = States.SETPOWER;
                    break;
                case States.SETPOWER:
                    powerLevel = (powerLevel >= 700 ? 50 : powerLevel+50);
                    myDisplay.ShowPower((int)powerLevel);
                    break;
            }
        }

        public void OnTimePressed(object sender, EventArgs e)
        {
            switch (myState)
            {
                case States.SETPOWER:
                    myDisplay.ShowTime((int)time, 0);
                    myState = States.SETTIME;
                    break;
                case States.SETTIME:
                    time += 1;
                    if (time > 60)                                      //Added limiter to time (max one hour)
                    {
                        time = 1;
                    }
                    myDisplay.ShowTime((int)time, 0);
                    break;
            }
        }

        public void OnStartCancelPressed(object sender, EventArgs e)
        {
            switch (myState)
            {
                case States.SETPOWER:
                    powerLevel = 50;
                    time = 1;
                    myLight.TurnOff();
                    myDisplay.Clear();
                    myState = States.READY;
                    break;
                case States.SETTIME:
                    myLight.TurnOn();
                    myCooker.StartCooking((int)powerLevel, (int)time *60);
                    myState = States.COOKING;
                    break;
                case States.COOKING:
                    powerLevel = 50;
                    time = 1;
                    myCooker.Stop();
                    myLight.TurnOff();
                    myDisplay.Clear();
                    myState = States.READY;
                    break;
            }
        }

        public void OnDoorOpened(object sender, EventArgs e)
        {
            switch (myState)
            {
                case States.READY:
                    myLight.TurnOn();
                    myState = States.DOOROPEN;
                    break;
                case States.SETPOWER:
                    powerLevel = 50;
                    myLight.TurnOn();
                    myDisplay.Clear();
                    myState = States.DOOROPEN;
                    break;
                case States.SETTIME:
                    powerLevel = 50;
                    time = 1;
                    myLight.TurnOn();
                    myDisplay.Clear();
                    myState = States.DOOROPEN;
                    break;
                case States.COOKING:
                    myCooker.Stop();
                    myDisplay.Clear();                              // Added Display.Clear
                    powerLevel = 50;
                    time = 1;
                    myState = States.DOOROPEN;
                    break;
            }
        }

        public void OnDoorClosed(object sender, EventArgs e)
        {
            switch (myState)
            {
                case States.DOOROPEN:
                    myLight.TurnOff();
                    myState = States.READY;
                    break;
            }
        }

        public void CookingIsDone()
        {
            switch (myState)
            {
                case States.COOKING:
                    powerLevel = 50;
                    time = 1;
                    myDisplay.Clear();
                    myLight.TurnOff();
                    // Beep 3 times
                    myState = States.READY;
                    break;
            }
        }
    }
}