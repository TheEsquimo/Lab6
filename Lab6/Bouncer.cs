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
        int fastestGuestLetInTime = 3;
        int slowestGuestLetInTime = 10;
        Random random = new Random();
        
        public Bouncer(MainWindow mainWindow)
        {
            TheMainWindow = mainWindow;
        }
        public void LetGuestsIn()
        {
            Task.Run(() =>
            {
                if (TheMainWindow.timeTillBarCloses <= 0)
                {
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        TheMainWindow.guestListBox.Items.Insert(0, "Bouncer walks home");
                    });
                    return;
                }
                Thread.Sleep((random.Next(fastestGuestLetInTime, slowestGuestLetInTime)) * 100);
                int randomNameNumber = random.Next(TheMainWindow.guestNames.Count);
                string nameOfNewGuest = TheMainWindow.guestNames[randomNameNumber];
                Guest newGuest = new Guest(nameOfNewGuest, TheMainWindow);
                TheMainWindow.guests.Add(newGuest);
                TheMainWindow.guestsWaitingForBeer.Enqueue(newGuest);
                newGuest.Start();
                LetGuestsIn();
            });
        }
    }
}
