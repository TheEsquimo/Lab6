using System;
using System.Threading;
using System.Threading.Tasks;
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
        const int minDrinkTime = 10000;
        const int maxDrinkTime = 20000;
        
        public string Name { get; set; }
        public string Message { get; set; }
        internal Glass HeldGlass { get; set; }
        internal Seat MySeat { get; set; }
        public MainWindow TheMainWindow { set; get; }
        public Guest(string name, MainWindow mainWindow)
        {
            Name = name;
            HeldGlass = null;
            TheMainWindow = mainWindow;
        }

        internal void Start()
        {
            Task thisTask = Task.Run(() =>
            {
                EnterBar();
                SearchForASeat();
                TakeASeat();
                LeaveBar();
            });
            TheMainWindow.activeTasks.Add(thisTask);
        }
        internal void EnterBar()
        {
            Message = $"{Name}: {enterBarMessage}";
            TheMainWindow.ListBoxMessage(TheMainWindow.guestListBox, Message);
            Thread.Sleep(timeToGetToBar / TheMainWindow.simulationSpeed);
            while (HeldGlass == null) { Thread.Sleep(250); }
        }
        internal void SearchForASeat()
        {
            TheMainWindow.guestsWaitingForSeat.Enqueue(this);
            Message = $"{Name}: {searchForSeatMessage}";
            TheMainWindow.ListBoxMessage(TheMainWindow.guestListBox, Message);
            while (true)
            {
                foreach (var seat in TheMainWindow.seats)
                {
                    Guest tempGuest;
                    TheMainWindow.guestsWaitingForSeat.TryPeek(out tempGuest);
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
            Thread.Sleep(timeToGetToSeat / TheMainWindow.simulationSpeed);
            TheMainWindow.availableSeats--;
            TheMainWindow.LabelMessage(TheMainWindow.availableSeatsAmountLabel, $"Available seats: {TheMainWindow.availableSeats}" +
                                             $"\nTotal: {TheMainWindow.seatAmount}");
            Guest tempGuest = this;
            TheMainWindow.guestsWaitingForSeat.TryDequeue(out tempGuest);
            Message = $"{Name}: {sitDownMessage}";
            TheMainWindow.ListBoxMessage(TheMainWindow.guestListBox, Message);
            int drinkTime = random.Next((minDrinkTime), (maxDrinkTime));
            Thread.Sleep(drinkTime / TheMainWindow.simulationSpeed);
        }
        internal void LeaveBar()
        {
            Message = $"{Name}: {finishedDrinkMessage}";
            TheMainWindow.ListBoxMessage(TheMainWindow.guestListBox, Message);
            MySeat.Guest = null;
            TheMainWindow.dirtyGlasses.TryAdd(HeldGlass);
            HeldGlass = null;
            Guest tempGuest = this;
            TheMainWindow.guests.TryTake(out tempGuest);
            TheMainWindow.availableSeats++;
            TheMainWindow.LabelMessage(TheMainWindow.availableSeatsAmountLabel, $"Available seats: {TheMainWindow.availableSeats}" +
                                             $"\nTotal: {TheMainWindow.seatAmount}");
            TheMainWindow.LabelMessage(TheMainWindow.guestAmountLabel, $"Guests: {TheMainWindow.guests.Count}");
        }
    }
}