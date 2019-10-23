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
        int collectDishesTime;
        int cleanDishesTime;
        string collectingDishesMessage = "Collecting dishes";
        string cleaningDishesMessage = "Cleaning dishes";
        string finishedCleaningMessage = "Put glasses back on shelf";
        string goHomeMessage = "Waiter goes home";
        
        public Waiter(MainWindow mainWindow, int theCollectDishesTime = 10000, int theCleanDishesTime = 15000)
        {
            TheMainWindow = mainWindow;
            int collectDishesTime = theCollectDishesTime;
            int cleanDishesTime = theCleanDishesTime;
        }

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
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.waiterListBox.Items.Insert(0, collectingDishesMessage);
            });
            while (TheMainWindow.dirtyGlasses.Count <= 0)
            {
                Thread.Sleep(250);
            }
            Thread.Sleep(collectDishesTime * TheMainWindow.simulationSpeed);
            foreach(Glass glass in TheMainWindow.dirtyGlasses)
            {
                Glass dirtyGlass;
                TheMainWindow.dirtyGlasses.TryTake(out dirtyGlass);
                dirtyGlasses.TryAdd(dirtyGlass);
            }
        }

        private void CleanDishes()
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.waiterListBox.Items.Insert(0, cleaningDishesMessage);
            });
            Thread.Sleep(cleanDishesTime * TheMainWindow.simulationSpeed);
            foreach(Glass glass in dirtyGlasses)
            {
                Glass cleanedGlass;
                dirtyGlasses.TryTake(out cleanedGlass);
                TheMainWindow.glassShelf.TryAdd(cleanedGlass);
            }
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.waiterListBox.Items.Insert(0, finishedCleaningMessage);
            });
        }

        private void GoHome()
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TheMainWindow.waiterListBox.Items.Insert(0, goHomeMessage);
            });
        }
    }
}