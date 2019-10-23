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
        int minDrinkTime = 10;
        int maxDrinkTime = 20;
        Random random = new Random();
        public string Name { get; set; }
        public string Message { get; set; }
        internal Glass HeldGlass { get; set; }
        internal Chair MyChair { get; set; }
        public MainWindow TheMainWindow { set; get; }
        public Guest(string name, MainWindow mainWindow)
        {
            Name = name;
            HeldGlass = new Glass();
            TheMainWindow = mainWindow;
        }

        internal void Start()
        {
            Task.Run(() =>
            {
                EnterBar();
                SearchForACHair();
                TakeASeat();
                LeaveBar();
            });
        }
        internal void EnterBar()
        {
            Message = $"{Name}: {enterBarMessage}";
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.guestListBox.Items.Insert(0, Message);
            });
            Thread.Sleep(1000);
            while (HeldGlass == null) { Thread.Sleep(250); }
            var tempGuest = this;
            TheMainWindow.guestsWaitingForBeer.TryDequeue(out tempGuest);
        }
        internal void SearchForACHair()
        {
            Message = $"{Name}: {searchForChairMessage}";
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.guestListBox.Items.Refresh();
            });
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
                        Thread.Sleep(4000);
                        return;
                    }
                }
                Thread.Sleep(250);
            }
        }
        internal void TakeASeat()
        {
            Message = $"{Name}: {sitDownMessage}";
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.guestListBox.Items.Refresh();
            });
            int drinkTime = random.Next((minDrinkTime * 1000), (maxDrinkTime * 1000));
            Thread.Sleep(drinkTime);
        }
        internal void LeaveBar()
        {
            Message = $"{Name}: {finishedDrinkMessage}";
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.guestListBox.Items.Refresh();
            });
            MyChair.Guest = null;
            TheMainWindow.dirtyGlasses.TryAdd(HeldGlass);
            HeldGlass = null;
        }
    }
}