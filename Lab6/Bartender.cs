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
        bool waitForCustomerMessageSent = false;
        const string waitForCustomerMessage = "Waiting for customer";
        const string fetchingGlassMessage = "Getting a glass from the shelf";
        const string fillingGlassMessage = "Pouring a glass of beer";
        const string goHomeMessage = "Bartender goes home";
        Glass heldGlass;

        public Bartender(MainWindow mainWindow, int theFetchGlassTime = 3000, int thePourBeerTime = 3000)
        {
            TheMainWindow = mainWindow;
            fetchGlassTime = theFetchGlassTime;
            pourBeerTime = thePourBeerTime;
        }

        public void Start()
        {
            Task thisTask = Task.Run(() => 
            {
                while (TheMainWindow.timeTillBarCloses > 0 || TheMainWindow.guests.Count > 0)
                {
                    if (!WaitForCustomer())
                    {
                        waitForCustomerMessageSent = false;
                        FetchGlass();
                        PourBeer();
                    }
                }
                GoHome();
            });
            TheMainWindow.activeTasks.Add(thisTask);
        }

        private bool WaitForCustomer()
        {
            if (TheMainWindow.guestsWaitingForBeer.IsEmpty)
            {
                if (!waitForCustomerMessageSent)
                {
                    TheMainWindow.ListBoxMessage(TheMainWindow.bartenderListBox, waitForCustomerMessage);
                    waitForCustomerMessageSent = true;
                }
                Thread.Sleep(250);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FetchGlass()
        {
            while (TheMainWindow.glassShelf.Count <= 0)
            {
                Thread.Sleep(250);
            }
            TheMainWindow.ListBoxMessage(TheMainWindow.bartenderListBox, fetchingGlassMessage);
            Thread.Sleep(fetchGlassTime / TheMainWindow.simulationSpeed);
            heldGlass = TheMainWindow.glassShelf.Take();
            TheMainWindow.LabelMessage(TheMainWindow.glassesAmountLabel, $"Available glasses: {TheMainWindow.glassShelf.Count}" +
                                                                         $"\nTotal: {TheMainWindow.glassAmount}");
        }

        private void PourBeer()
        {
            Guest guestToRecieveBeer;
            TheMainWindow.guestsWaitingForBeer.TryDequeue(out guestToRecieveBeer);
            TheMainWindow.ListBoxMessage(TheMainWindow.bartenderListBox, $"{fillingGlassMessage} for {guestToRecieveBeer.Name}");
            Thread.Sleep(pourBeerTime / TheMainWindow.simulationSpeed);
            guestToRecieveBeer.HeldGlass = heldGlass;
            heldGlass = null;
        }

        private void GoHome()
        {
            TheMainWindow.ListBoxMessage(TheMainWindow.bartenderListBox, goHomeMessage);
        }
    }
}