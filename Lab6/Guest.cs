using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Lab6
{
    public class Guest
    {
        const string enterBarMessage = "Entering the pub, heading to the bar";
        const string searchForSeatMessage = "Searching for a free seat";
        const string sitDownMessage = "Sitting down and drinking beer";
        const string finishedDrinkMessage = "Finished drink, leaving bar";
        Random random = new Random();
        const int timeToGetToBar = 1000;
        const int timeToGetToSeat = 4000;
        const int minDrinkTime = 20000;
        const int maxDrinkTime = 30000;
        private Action<ListBox, string> listBoxMessage;
        private Action<Label, string> labelMessage;
        
        public string Name { get; set; }
        public string Message { get; set; }
        internal Glass HeldGlass { get; set; }
        internal Seat MySeat { get; set; }
        public Pub ThePub { get; }
        public MainWindow TheMainWindow { get; }
        public Guest(string name, Pub pub, MainWindow mainWindow, Action<ListBox, string> theListBoxMessage, Action<Label, string> theLabelMessage)
        {
            Name = name;
            HeldGlass = null;
            ThePub = pub;
            TheMainWindow = mainWindow;
            listBoxMessage = theListBoxMessage;
            labelMessage = theLabelMessage;
        }

        internal void Start()
        {
            Task guestTask = Task.Run(() =>
            {
                EnterBar();
                SearchForASeat();
                TakeASeat();
                LeaveBar();
            });
            ThePub.activeTasks.Add(guestTask);
        }
        internal void EnterBar()
        {
            ThePub.guestsWaitingForBeer.Enqueue(this);
            Message = $"{Name}: {enterBarMessage}";
            listBoxMessage(TheMainWindow.guestListBox, Message);
            Thread.Sleep(timeToGetToBar / ThePub.simulationSpeed);
            while (HeldGlass == null) { Thread.Sleep(250); }
        }
        internal void SearchForASeat()
        {
            ThePub.guestsWaitingForSeat.Enqueue(this);
            Message = $"{Name}: {searchForSeatMessage}";
            listBoxMessage(TheMainWindow.guestListBox, Message);
            while (true)
            {
                foreach (var seat in ThePub.seats)
                {
                    Guest tempGuest;
                    ThePub.guestsWaitingForSeat.TryPeek(out tempGuest);
                    if (seat.Guest == null && this == tempGuest)
                    {
                        seat.Guest = this;
                        MySeat = seat;
                        return;
                    }
                }
                Thread.Sleep(250);
            }
        }
        internal void TakeASeat()
        {
            Thread.Sleep(timeToGetToSeat / ThePub.simulationSpeed);
            ThePub.availableSeats--;
            labelMessage(TheMainWindow.availableSeatsAmountLabel, $"Available seats: {ThePub.availableSeats}" +
                                             $"\nTotal: {ThePub.seatAmount}");
            Guest tempGuest = this;
            ThePub.guestsWaitingForSeat.TryDequeue(out tempGuest);
            Message = $"{Name}: {sitDownMessage}";
            listBoxMessage(TheMainWindow.guestListBox, Message);
            int drinkTime = random.Next((minDrinkTime), (maxDrinkTime));
            Thread.Sleep(drinkTime / ThePub.simulationSpeed);
        }
        internal void LeaveBar()
        {
            Message = $"{Name}: {finishedDrinkMessage}";
            listBoxMessage(TheMainWindow.guestListBox, Message);
            MySeat.Guest = null;
            ThePub.dirtyGlasses.TryAdd(HeldGlass);
            HeldGlass = null;
            Guest tempGuest = this;
            ThePub.guests.TryTake(out tempGuest);
            ThePub.availableSeats++;
            labelMessage(TheMainWindow.availableSeatsAmountLabel, $"Available seats: {ThePub.availableSeats}" +
                                             $"\nTotal: {ThePub.seatAmount}");
            labelMessage(TheMainWindow.guestAmountLabel, $"Guests: {ThePub.guests.Count}");
        }
    }
}