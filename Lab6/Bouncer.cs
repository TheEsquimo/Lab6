using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    class Bouncer
    {
        public MainWindow TheMainWindow { set; get; }
        int fastestGuestLetInTime = 3;
        int slowestGuestLetInTime = 10;
        Random random = new Random();

        public void LetGuestsIn()
        {
            if (TheMainWindow.timeTillBarCloses <= 0)
            {
                TheMainWindow.patronListBox.Items.Insert(0, "Bouncer walks home");
                return;
            }
            Thread.Sleep((random.Next(fastestGuestLetInTime, slowestGuestLetInTime)) * 1000);
            int randomNameNumber = random.Next(TheMainWindow.guestNames.Count);
            string nameOfNewGuest = TheMainWindow.guestNames[randomNameNumber];
            Guest newGuest = new Guest(nameOfNewGuest);
            TheMainWindow.guestsWaitingForBeer.Enqueue(newGuest);
            LetGuestsIn();
        }
    }
}
