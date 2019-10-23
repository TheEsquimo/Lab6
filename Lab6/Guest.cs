using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Lab6
{
    public class Guest
    {
        string enterBarMessage = "Entering the pub, heading to the bar";
        string searchForChairMessage = "Searching for a free chair";
        string sitDownMessage = "Sitting down and drinking beer";
        string finishedDrinkMessage = "Finished drink, leaving bar";
        Random random = new Random();
        int timeToGetToBar = 1000;
        int timeToGetToChair = 4000;
        int minDrinkTime = 10000;
        int maxDrinkTime = 20000;
        
        public string Name { get; set; }
        public string Message { get; set; }
        internal Glass HeldGlass { get; set; }
        internal Seat MyChair { get; set; }
        public MainWindow TheMainWindow { set; get; }
        public Guest(string name, MainWindow mainWindow)
        {
            Name = name;
            HeldGlass = null;
            TheMainWindow = mainWindow;
        }

        internal void Start()
        {
            Task.Run(() =>
            {
                EnterBar();
                SearchForAChair();
                TakeASeat();
                LeaveBar();
            });
        }
        internal void EnterBar()
        {
            Message = $"{Name}: {enterBarMessage}";
            TheMainWindow.ListBoxMessage(TheMainWindow.guestListBox, Message);
            Thread.Sleep(timeToGetToBar / TheMainWindow.simulationSpeed);
            while (HeldGlass == null) { Thread.Sleep(250); }
        }
        internal void SearchForAChair()
        {
            TheMainWindow.guestsWaitingForSeat.Enqueue(this);
            Message = $"{Name}: {searchForChairMessage}";
            TheMainWindow.ListBoxMessage(TheMainWindow.guestListBox, Message);
            while (true)
            {
                foreach (var chair in TheMainWindow.chairs)
                {
                    Guest tempGuest;
                    TheMainWindow.guestsWaitingForSeat.TryPeek(out tempGuest);
                    if (chair.Guest == null && this == tempGuest)
                    {
                        chair.Guest = this;
                        MyChair = chair;
                        return;
                    }
                }
                Thread.Sleep(250);
            }
        }
        internal void TakeASeat()
        {
            Thread.Sleep(timeToGetToChair / TheMainWindow.simulationSpeed);
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
            MyChair.Guest = null;
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