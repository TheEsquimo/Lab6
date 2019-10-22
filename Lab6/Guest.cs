using System;
using System.Threading;

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
        internal Glass HeldGlass { get; set; }
        internal Chair MyChair { get; set; }
        public MainWindow TheMainWindow { set; get; }
        public Guest(string name)
        {
            Name = name;
        }

        internal void EnterBar()
        {
            TheMainWindow.guestListBox.Items.Insert(0, enterBarMessage);
            Thread.Sleep(1000);
            while (HeldGlass == null) { Thread.Sleep(250); }
        }
        internal void SearchForACHair()
        {
            TheMainWindow.guestListBox.Items.Insert(0, searchForChairMessage);
            while (true)
            {
                foreach (var chair in TheMainWindow.chairs)
                {
                    if (chair.Guest == null)
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
        internal void TakeSeat()
        {
            TheMainWindow.guestListBox.Items.Insert(0, sitDownMessage);
            int drinkTime = random.Next((minDrinkTime * 1000), (maxDrinkTime * 1000));
            Thread.Sleep(drinkTime);
        }
        internal void LeaveBar()
        {
            TheMainWindow.guestListBox.Items.Insert(0, finishedDrinkMessage);
            MyChair.Guest = null;
            TheMainWindow.dirtyGlasses.TryAdd(HeldGlass);
            HeldGlass = null;
        }
    }
}