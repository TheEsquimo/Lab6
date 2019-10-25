using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Lab6
{
    class Bartender
    {
        public MainWindow TheMainWindow { get; set; }
        bool waitForCustomerMessageSent = false;
        const int fetchGlassTime = 3000;
        const int pourBeerTime = 3000;
        const string waitForCustomerMessage = "Waiting for customer";
        const string fetchingGlassMessage = "Getting a glass from the shelf";
        const string fillingGlassMessage = "Pouring a glass of beer";
        const string goHomeMessage = "Bartender goes home";
        Glass heldGlass;

        public Bartender(MainWindow mainWindow)
        {
            TheMainWindow = mainWindow;
        }

        public void Start()
        {
            Task bartenderTask = Task.Run(() => 
            {
                while (TheMainWindow.timeTillBarCloses > 0 || TheMainWindow.guests.Count > 0)
                {
                    WaitForCustomer();
                    FetchGlass();
                    PourBeer();
                }
                GoHome();
            });
            TheMainWindow.activeTasks.Add(bartenderTask);
        }

        private void WaitForCustomer()
        {
            TheMainWindow.ListBoxMessage(TheMainWindow.bartenderListBox, waitForCustomerMessage);
            while (TheMainWindow.guestsWaitingForBeer.IsEmpty)
            {
                Thread.Sleep(250);
            }
        }

        private void FetchGlass()
        {
            while (TheMainWindow.glassShelf.Count <= 0)
            {
                Thread.Sleep(250);
            }
            TheMainWindow.ListBoxMessage(TheMainWindow.bartenderListBox, fetchingGlassMessage);
            Thread.Sleep(fetchGlassTime * TheMainWindow.simulationSpeed);
            heldGlass = TheMainWindow.glassShelf.Take();
        }

        private void PourBeer()
        {
            TheMainWindow.ListBoxMessage(TheMainWindow.bartenderListBox, fillingGlassMessage);
            Thread.Sleep(pourBeerTime * TheMainWindow.simulationSpeed);
            Guest guestToRecieveBeer;
            TheMainWindow.guestsWaitingForBeer.TryDequeue(out guestToRecieveBeer);
            guestToRecieveBeer.HeldGlass = heldGlass;
            heldGlass = null;
        }

        private void GoHome()
        {
            TheMainWindow.ListBoxMessage(TheMainWindow.bartenderListBox, goHomeMessage);
        }
    }
}