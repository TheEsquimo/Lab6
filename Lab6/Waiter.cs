using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Lab6
{
    internal class Waiter
    {
        public Pub ThePub { get; }
        private Action<object, string> listBoxMessage;
        private Action<Labell, string> labelMessage;
        BlockingCollection<Glass> dirtyGlasses = new BlockingCollection<Glass>();
        const int collectDishesTime = 10000;
        const int cleanDishesTime = 15000;
        const string lookingForDishesMessage = "Looking for dishes";
        const string collectingDishesMessage = "Collecting dishes";
        const string cleaningDishesMessage = "Cleaning dishes";
        const string finishedCleaningMessage = "Put glasses back on shelf";
        const string goHomeMessage = "Waiter goes home";
        
        public Waiter(Pub pub, Action<object, string> theListBoxMessage, Action<Labell, string> theLabelMessage)
        {
            ThePub = pub;
            listBoxMessage = theListBoxMessage;
            labelMessage = theLabelMessage;
        }

        public void Start()
        {
            Task waiterTask = Task.Run(() =>
            {
            while (ThePub.timeTillBarCloses > 0 || ThePub.guests.Count > 0 || ThePub.dirtyGlasses.Count > 0)
                {
                    CollectDishes();
                    CleanDishes();
                }
                GoHome();
            });
            ThePub.activeTasks.Add(waiterTask);
        }

        private void CollectDishes()
        {
            listBoxMessage(this, lookingForDishesMessage);
            while (ThePub.dirtyGlasses.Count <= 0)
            {
                if (ThePub.timeTillBarCloses <= 0 && ThePub.guests.Count <= 0) { return; }
                Thread.Sleep(250);
            }
            listBoxMessage(this, collectingDishesMessage);
            Thread.Sleep(collectDishesTime / ThePub.simulationSpeed);
            foreach(Glass glass in ThePub.dirtyGlasses)
            {
                Glass dirtyGlass;
                ThePub.dirtyGlasses.TryTake(out dirtyGlass);
                dirtyGlasses.TryAdd(dirtyGlass);
            }
        }

        private void CleanDishes()
        {
            if (dirtyGlasses.Count > 0)
            {
                listBoxMessage(this, cleaningDishesMessage);
                Thread.Sleep(cleanDishesTime / ThePub.simulationSpeed);
                foreach(Glass glass in dirtyGlasses)
                {
                    Glass cleanedGlass = null;
                    dirtyGlasses.TryTake(out cleanedGlass);
                    ThePub.glassShelf.TryAdd(cleanedGlass);
                }
                listBoxMessage(this, finishedCleaningMessage);
                labelMessage(Labell.GlassesAvailable, $"Available glasses: {ThePub.glassShelf.Count}" +
                                                                             $"\nTotal: {ThePub.glassAmount}");
            }
        }

        private void GoHome()
        {
            listBoxMessage(this, goHomeMessage);
        }
    }
}