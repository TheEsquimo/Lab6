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
            fetchGlassTime = theFetchGlassTime;
            pourBeerTime = thePourBeerTime;
        }

        public void Start()
        {
            Task.Run(() => 
            {
                while (TheMainWindow.timeTillBarCloses > 0 || TheMainWindow.guests.Count > 0)
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
            TheMainWindow.LabelMessage(TheMainWindow.glassesAmountLabel, $"Available glasses: {TheMainWindow.glassShelf.Count}" +
                                                                         $"\nTotal: {TheMainWindow.glassAmount}");
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