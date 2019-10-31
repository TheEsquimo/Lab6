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
        public Pub ThePub { get; }
        private Action<object, string> listBoxMessage;
        private Action<Labell, string> labelMessage;
        bool waitForCustomerMessageSent = false;
        const int fetchGlassTime = 3000;
        const int pourBeerTime = 3000;
        const string waitForCustomerMessage = "Waiting for customer";
        const string fetchingGlassMessage = "Getting a glass from the shelf";
        const string fillingGlassMessage = "Pouring a glass of beer";
        const string goHomeMessage = "Bartender goes home";
        Glass heldGlass;

        public Bartender(Pub pub, Action<object, string> theListBoxMessage, Action<Labell, string> theLabelMessage)
        {
            ThePub = pub;
            listBoxMessage = theListBoxMessage;
            labelMessage = theLabelMessage;
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
                    listBoxMessage(this, waitForCustomerMessage);
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
            listBoxMessage(this, fetchingGlassMessage);
            Thread.Sleep(fetchGlassTime / ThePub.simulationSpeed);
            heldGlass = ThePub.glassShelf.Take();
            labelMessage(Labell.GlassesAvailable, $"Available glasses: {ThePub.glassShelf.Count}" +
                                                                         $"\nTotal: {ThePub.glassAmount}");
        }

        private void PourBeer()
        {
            Guest guestToRecieveBeer;
            ThePub.guestsWaitingForBeer.TryDequeue(out guestToRecieveBeer);
            listBoxMessage(this, $"{fillingGlassMessage} for {guestToRecieveBeer.Name}");
            Thread.Sleep(pourBeerTime / ThePub.simulationSpeed);
            guestToRecieveBeer.HeldGlass = heldGlass;
            heldGlass = null;
        }

        private void GoHome()
        {
            listBoxMessage(this, goHomeMessage);
        }
    }
}