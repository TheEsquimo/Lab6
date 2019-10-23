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
        int fetchGlassTime;
        int pourBeerTime;
        string waitForCustomerMessage = "Waiting for customer";
        string fetchingGlassMessage = "Getting a glass from the shelf";
        string fillingGlassMessage = "Pouring a glass of beer";
        string goHomeMessage = "Bartender goes home";
        Glass heldGlass;

        public Bartender(MainWindow mainWindow, int theFetchGlassTime = 3000, int thePourBeerTime = 3000)
        {
            TheMainWindow = mainWindow;
            int fetchGlassTime = theFetchGlassTime;
            int pourBeerTime = thePourBeerTime;
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (TheMainWindow.timeTillBarCloses > 0 && TheMainWindow.guests.Count > 0)
                {
                    WaitForCustomer();
                    FetchGlass();
                    PourBeer();
                }
                GoHome();
            });
        }

        private void WaitForCustomer()
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.bartenderListBox.Items.Insert(0, waitForCustomerMessage);
            });
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
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.bartenderListBox.Items.Insert(0, fetchingGlassMessage);
            });
            Thread.Sleep(fetchGlassTime * TheMainWindow.simulationSpeed);
            heldGlass = TheMainWindow.glassShelf.Take();
        }

        private void PourBeer()
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.bartenderListBox.Items.Insert(0, fillingGlassMessage);
            });
            Thread.Sleep(pourBeerTime * TheMainWindow.simulationSpeed);
            Guest guestToRecieveBeer;
            TheMainWindow.guestsWaitingForBeer.TryDequeue(out guestToRecieveBeer);
            guestToRecieveBeer.HeldGlass = heldGlass;
            heldGlass = null;
        }

        private void GoHome()
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.bartenderListBox.Items.Insert(0, goHomeMessage);
            });
        }
    }
}