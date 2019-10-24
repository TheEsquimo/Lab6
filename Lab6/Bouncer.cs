using System;
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
        int fastestGuestLetInTime = 3000;
        int slowestGuestLetInTime = 10000;
        Random random = new Random();
        
        public Bouncer(MainWindow mainWindow)
        {
            TheMainWindow = mainWindow;
        }
        public void LetGuestsIn()
        {
            Task thisTask = Task.Run(() =>
            {
                if (TheMainWindow.timeTillBarCloses <= 0)
                {
                    TheMainWindow.ListBoxMessage(TheMainWindow.guestListBox, "Bouncer goes home");
                    return;
                }
                Thread.Sleep((random.Next(fastestGuestLetInTime, slowestGuestLetInTime)) / TheMainWindow.simulationSpeed);
                int randomNameNumber = random.Next(TheMainWindow.guestNames.Count);
                string nameOfNewGuest = TheMainWindow.guestNames[randomNameNumber];
                Guest newGuest = new Guest(nameOfNewGuest, TheMainWindow);
                TheMainWindow.guests.Add(newGuest);
                TheMainWindow.guestsWaitingForBeer.Enqueue(newGuest);
                TheMainWindow.LabelMessage(TheMainWindow.guestAmountLabel, $"Guests: {TheMainWindow.guests.Count}");
                newGuest.Start();
                LetGuestsIn();
            });
            TheMainWindow.activeTasks.Add(thisTask);
        }
    }
}
