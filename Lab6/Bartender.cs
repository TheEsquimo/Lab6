using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Lab6
{
    class Bartender
    {
        public MainWindow TheMainWindow { get; }
        public Pub ThePub { get; }
        private Action<ListBox, string> listBoxMessage;
        bool waitForCustomerMessageSent = false;
        const int fetchGlassTime = 3000;
        const int pourBeerTime = 3000;
        const string waitForCustomerMessage = "Waiting for customer";
        const string fetchingGlassMessage = "Getting a glass from the shelf";
        const string fillingGlassMessage = "Pouring a glass of beer";
        const string goHomeMessage = "Bartender goes home";
        Glass heldGlass;

        public Bartender(MainWindow mainWindow, Pub pub, Action<ListBox, string> theListBoxMessage)
        {
            TheMainWindow = mainWindow;
            ThePub = pub;
            listBoxMessage = theListBoxMessage;
        }

        public void Start()
        {
            Task bartenderTask = Task.Run(() => 
            {
                while (ThePub.timeTillBarCloses > 0 || ThePub.guests.Count > 0)
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
            ThePub.activeTasks.Add(bartenderTask);
        }

        private bool WaitForCustomer()
        {
            if (ThePub.guestsWaitingForBeer.IsEmpty)
            {
                if (!waitForCustomerMessageSent)
                {
                    listBoxMessage(TheMainWindow.bartenderListBox, waitForCustomerMessage);
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
            while (ThePub.glassShelf.Count <= 0)
            {
                Thread.Sleep(250);
            }
            listBoxMessage(TheMainWindow.bartenderListBox, fetchingGlassMessage);
            Thread.Sleep(fetchGlassTime / ThePub.simulationSpeed);
            heldGlass = ThePub.glassShelf.Take();
            listBoxMessage(TheMainWindow.bartenderListBox, $"Available glasses: {ThePub.glassShelf.Count}" +
                                                                         $"\nTotal: {ThePub.glassAmount}");
        }

        private void PourBeer()
        {
            Guest guestToRecieveBeer;
            ThePub.guestsWaitingForBeer.TryDequeue(out guestToRecieveBeer);
            listBoxMessage(TheMainWindow.bartenderListBox, $"{fillingGlassMessage} for {guestToRecieveBeer.Name}");
            Thread.Sleep(pourBeerTime / ThePub.simulationSpeed);
            guestToRecieveBeer.HeldGlass = heldGlass;
            heldGlass = null;
        }

        private void GoHome()
        {
            listBoxMessage(TheMainWindow.bartenderListBox, goHomeMessage);
        }
    }
}