using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    class Bartender
    {
        public MainWindow TheMainWindow { get; set; }
        int fetchGlassTime = 3000;
        int pourBeerTime = 3000;
        Glass heldGlass;

        public void Start()
        {
            Task.Run(() =>
            {
                while (TheMainWindow.timeTillBarCloses > 0) //And no more guests exists
                {
                    WaitForCustomer();
                    FetchGlass();
                    PourBeer();
                }
                GoHome();
            });
        }

        private void GoHome()
        {
            TheMainWindow.bartenderListBox.Items.Insert(0, "Bartender goes home");
        }

        private void PourBeer()
        {
            TheMainWindow.bartenderListBox.Items.Insert(0, "Pouring a glass of beer");
            Thread.Sleep(pourBeerTime * TheMainWindow.simulationSpeed);
            Guest guestToRecieveBeer;
            TheMainWindow.guestsWaitingForBeer.TryDequeue(out guestToRecieveBeer);
            //Give heldBeer to guest
        }

        private void FetchGlass()
        {
            while (TheMainWindow.glassShelf.Count <= 0)
            {
                Thread.Sleep(500);
            }
            TheMainWindow.bartenderListBox.Items.Insert(0, "Getting a glass from the shelf");
            Thread.Sleep(fetchGlassTime * TheMainWindow.simulationSpeed);
            heldGlass = TheMainWindow.glassShelf.Take();
        }

        private void WaitForCustomer()
        {
            TheMainWindow.bartenderListBox.Items.Insert(0, "Waiting for customer");
            while (TheMainWindow.guestsWaitingForBeer.IsEmpty)
            {
                Thread.Sleep(500);
            }
        }
    }
}