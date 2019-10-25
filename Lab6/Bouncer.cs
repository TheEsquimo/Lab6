﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Lab6
{
    class Bouncer
    {
        public MainWindow TheMainWindow { set; get; }
        const int fastestGuestLetInTime = 3000;
        const int slowestGuestLetInTime = 10000;
        Random random = new Random();
        
        public Bouncer(MainWindow mainWindow)
        {
            TheMainWindow = mainWindow;
        }
        public void Work()
        {
            Task bouncerTask = Task.Run(() =>
            {
                if (TheMainWindow.timeTillBarCloses <= 0)
                {
                    TheMainWindow.ListBoxMessage(TheMainWindow.guestListBox, "Bouncer goes home");
                    return;
                }
                Thread.Sleep((random.Next(fastestGuestLetInTime, slowestGuestLetInTime)) / TheMainWindow.simulationSpeed);
                if (TheMainWindow.timeTillBarCloses > 0)
                {
                    if(TheMainWindow.SecondsBetweenDates(TheMainWindow.dateTimeStart ,DateTime.Now) > 20 && TheMainWindow.doBusLoad)
                    {
                        LetGuestsIn(15);
                        TheMainWindow.doBusLoad = false;
                    }
                    else
                    {
                        LetGuestsIn();
                    }
                }
                Work();
            });
            TheMainWindow.activeTasks.Add(bouncerTask);
        }
        void LetGuestsIn()
        {
            int randomNameNumber = random.Next(TheMainWindow.guestNames.Count);
            string nameOfNewGuest = TheMainWindow.guestNames[randomNameNumber];
            Guest newGuest = new Guest(nameOfNewGuest, TheMainWindow);
            TheMainWindow.guests.Add(newGuest);
            TheMainWindow.guestsWaitingForBeer.Enqueue(newGuest);
            TheMainWindow.LabelMessage(TheMainWindow.guestAmountLabel, $"Guests: {TheMainWindow.guests.Count}");
            newGuest.Start();
        }
        void LetGuestsIn(int amountOfGuests)
        {
            for (int i = 0; i < amountOfGuests; i++)
            {
                int randomNameNumber = random.Next(TheMainWindow.guestNames.Count);
                string nameOfNewGuest = TheMainWindow.guestNames[randomNameNumber];
                Guest newGuest = new Guest(nameOfNewGuest, TheMainWindow);
                TheMainWindow.guests.Add(newGuest);
                TheMainWindow.guestsWaitingForBeer.Enqueue(newGuest);
                newGuest.Start();
            }
            TheMainWindow.LabelMessage(TheMainWindow.guestAmountLabel, $"Guests: {TheMainWindow.guests.Count}");
        }
    }
}
