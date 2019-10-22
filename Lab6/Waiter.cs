using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lab6
{
    internal class Waiter
    {
        MainWindow TheMainWindow { get; set; }
        BlockingCollection<Glass> dirtyGlasses = new BlockingCollection<Glass>();
        int collectDishesTime = 10000;
        int cleanDishesTime = 15000;
        string collectingDishesMessage = "Collecting dishes";
        string cleaningDishesMessage = "Cleaning dishes";
        string finishedCleaningMessage = "Put glasses back on shelf";
        string goHomeMessage = "Waiter goes home";
        
        public void Start()
        {
            Task.Run(() =>
            {
                while (TheMainWindow.timeTillBarCloses > 0 && TheMainWindow.guests.Count > 0)
                {
                    CollectDishes();
                    CleanDishes();
                }
                GoHome();
            });
        }

        private void CollectDishes()
        {
            TheMainWindow.waiterListBox.Items.Insert(0, collectingDishesMessage);
            while (TheMainWindow.dirtyGlasses.Count <= 0)
            {
                Thread.Sleep(250);
            }
            Thread.Sleep(collectDishesTime);
            foreach(Glass glass in TheMainWindow.dirtyGlasses)
            {
                Glass dirtyGlass;
                TheMainWindow.dirtyGlasses.TryTake(out dirtyGlass);
                dirtyGlasses.TryAdd(dirtyGlass);
            }
        }

        private void CleanDishes()
        {
            TheMainWindow.waiterListBox.Items.Insert(0, cleaningDishesMessage);
            Thread.Sleep(cleanDishesTime);
            foreach(Glass glass in dirtyGlasses)
            {
                Glass cleanedGlass;
                dirtyGlasses.TryTake(out cleanedGlass);
                TheMainWindow.glassShelf.TryAdd(cleanedGlass);
            }
            TheMainWindow.waiterListBox.Items.Insert(0, finishedCleaningMessage);
        }

        private void GoHome()
        {
            TheMainWindow.waiterListBox.Items.Insert(0, goHomeMessage);
        }
    }
}