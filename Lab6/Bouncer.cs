using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Lab6
{
    class Bouncer
    {
        public MainWindow TheMainWindow { get; }
        public Pub ThePub { get; }
        private Action<ListBox, string> listBoxMessage;
        private Action<Label, string> labelMessage;
        const int fastestGuestLetInTime = 3000;
        const int slowestGuestLetInTime = 10000;
        Random random = new Random();
        
        public Bouncer(MainWindow mainWindow, Pub pub, Action<ListBox, string> theListBoxMessage, Action<Label, string> theLabelMessage)
        {
            TheMainWindow = mainWindow;
            ThePub = pub;
            listBoxMessage = theListBoxMessage;
            labelMessage = theLabelMessage;

        }
        public void Work()
        {
            Task bouncerTask = Task.Run(() =>
            {
                if (ThePub.timeTillBarCloses <= 0)
                {
                    listBoxMessage(TheMainWindow.guestListBox, "Bouncer goes home");
                    return;
                }
                Thread.Sleep((random.Next(fastestGuestLetInTime, slowestGuestLetInTime)) / ThePub.simulationSpeed);
                if (ThePub.timeTillBarCloses > 0)
                {
                    if(TheMainWindow.SecondsBetweenDates(ThePub.dateTimeStart ,DateTime.Now) > 20 && ThePub.doBusLoad)
                    {
                        ThePub.LetGuestsIn(15);
                        ThePub.doBusLoad = false;
                    }
                    else
                    {
                        ThePub.LetGuestsIn();
                    }
                }
                Work();
            });
            ThePub.activeTasks.Add(bouncerTask);
        }
    }
}
