using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Lab6
{
    internal class Waiter
    {
        internal MainWindow TheMainWindow { get; set; }
        BlockingCollection<Glass> dirtyGlasses = new BlockingCollection<Glass>();
        const int collectDishesTime = 10000;
        const int cleanDishesTime = 15000;
        const string lookingForDishesMessage = "Looking for dishes";
        const string collectingDishesMessage = "Collecting dishes";
        const string cleaningDishesMessage = "Cleaning dishes";
        const string finishedCleaningMessage = "Put glasses back on shelf";
        const string goHomeMessage = "Waiter goes home";
        
        public Waiter(MainWindow mainWindow)
        {
            TheMainWindow = mainWindow;
        }

        public void Start()
        {
            Task thisTask = Task.Run(() =>
            {
                while (TheMainWindow.timeTillBarCloses > 0 || TheMainWindow.guests.Count > 0)
                {
                    CollectDishes();
                    CleanDishes();
                }
                GoHome();
            });
            TheMainWindow.activeTasks.Add(thisTask);
        }

        private void CollectDishes()
        {
            TheMainWindow.ListBoxMessage(TheMainWindow.waiterListBox, lookingForDishesMessage);
            while (TheMainWindow.dirtyGlasses.Count <= 0)
            {
                if (TheMainWindow.timeTillBarCloses <= 0 && TheMainWindow.guests.Count <= 0) { return; }
                Thread.Sleep(250);
            }
            TheMainWindow.ListBoxMessage(TheMainWindow.waiterListBox, collectingDishesMessage);
            Thread.Sleep(collectDishesTime / TheMainWindow.simulationSpeed);
            foreach(Glass glass in TheMainWindow.dirtyGlasses)
            {
                Glass dirtyGlass;
                TheMainWindow.dirtyGlasses.TryTake(out dirtyGlass);
                dirtyGlasses.TryAdd(dirtyGlass);
            }
        }

        private void CleanDishes()
        {
            if (dirtyGlasses.Count > 0)
            {
                TheMainWindow.ListBoxMessage(TheMainWindow.waiterListBox, cleaningDishesMessage);
                Thread.Sleep(cleanDishesTime / TheMainWindow.simulationSpeed);
                foreach(Glass glass in dirtyGlasses)
                {
                    Glass cleanedGlass = null;
                    dirtyGlasses.TryTake(out cleanedGlass);
                    TheMainWindow.glassShelf.TryAdd(cleanedGlass);
                }
                TheMainWindow.ListBoxMessage(TheMainWindow.waiterListBox, finishedCleaningMessage);
                TheMainWindow.LabelMessage(TheMainWindow.glassesAmountLabel, $"Available glasses: {TheMainWindow.glassShelf.Count}" +
                                                                             $"\nTotal: {TheMainWindow.glassAmount}");
            }
        }

        private void GoHome()
        {
            TheMainWindow.ListBoxMessage(TheMainWindow.waiterListBox, goHomeMessage);
        }
    }
}