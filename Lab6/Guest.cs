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
        private Action<object, string> listBoxMessage;
        private Action<Labell, string> labelMessage;
        public Labell EditLabel { get; set; }

        public string Name { get; set; }
        public string Message { get; set; }
        internal Glass HeldGlass { get; set; }
        internal Seat MySeat { get; set; }
        public Pub ThePub { get; }
        public MainWindow TheMainWindow { get; }
        public Guest(string name, Pub pub, MainWindow mainWindow, Action<object, string> theListBoxMessage, Action<Labell, string> theLabelMessage)
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
            listBoxMessage(this, Message);
            Thread.Sleep(timeToGetToBar / ThePub.simulationSpeed);
            while (HeldGlass == null) { Thread.Sleep(250); }
        }
        internal void SearchForASeat()
        {
            ThePub.guestsWaitingForSeat.Enqueue(this);
            Message = $"{Name}: {searchForSeatMessage}";
            listBoxMessage(this, Message);
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
            labelMessage(Labell.SeatsAvailable, $"Available seats: {ThePub.availableSeats}" +
                                             $"\nTotal: {ThePub.seatAmount}");
            Guest tempGuest = this;
            ThePub.guestsWaitingForSeat.TryDequeue(out tempGuest);
            Message = $"{Name}: {sitDownMessage}";
            listBoxMessage(this, Message);
            int drinkTime = random.Next((minDrinkTime), (maxDrinkTime));
            Thread.Sleep(drinkTime / ThePub.simulationSpeed);
        }
        internal void LeaveBar()
        {
            Message = $"{Name}: {finishedDrinkMessage}";
            listBoxMessage(this, Message);
            MySeat.Guest = null;
            ThePub.dirtyGlasses.TryAdd(HeldGlass);
            HeldGlass = null;
            Guest tempGuest = this;
            ThePub.guests.TryTake(out tempGuest);
            ThePub.availableSeats++;
            labelMessage(Labell.SeatsAvailable, $"Available seats: {ThePub.availableSeats}" +
                                             $"\nTotal: {ThePub.seatAmount}");
            labelMessage(Labell.GuestAmount, $"Guests: {ThePub.guests.Count}");
        }
    }
}