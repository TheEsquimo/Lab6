﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool simulationInitiated = false;
        internal double timeTillBarCloses = 25;
        internal int glassAmount = 8;
        internal int seatAmount = 9;
        internal int availableSeats;
        DateTime dateTimeStart;
        DateTime dateTimeLastUpdate;
        internal int simulationSpeed = 1;
        internal BlockingCollection<Task> activeTasks;
        internal BlockingCollection<Glass> glassShelf;
        internal BlockingCollection<Glass> dirtyGlasses;
        internal BlockingCollection<Seat> seats;
        internal BlockingCollection<Guest> guests;
        internal ConcurrentQueue<Guest> guestsWaitingForBeer;
        internal ConcurrentQueue<Guest> guestsWaitingForSeat;
        internal List<string> guestNames = new List<string>
        {
            "Bert",
            "Berta",
            "Bertil",
            "Bertilda",
            "Boris",
            "Bertrand",
            "Putin",
            "Sonic",
            "Guy",
            "DudeGuyer",
            "Bert-Erik"
        };
        
        public MainWindow()
        {
            InitializeComponent();
            openCloseBarButton.Click += OnOpenCloseBarButtonClicked;
        }

        private void OnOpenCloseBarButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!simulationInitiated)
            {
                openCloseBarButton.Content = "Close bar";
                InitiateSimulation();
            }
            else
            {
                openCloseBarButton.IsEnabled = false;
                openCloseBarButton.Content = "Finishing up for the day";
                timeTillBarCloses = 0;
            }
        }

        public void InitiateSimulation()
        {
            simulationInitiated = true;
            activeTasks = new BlockingCollection<Task>();
            Random random = new Random();
            dateTimeStart = DateTime.Now;
            dateTimeLastUpdate = DateTime.Now;

            glassShelf = new BlockingCollection<Glass>();
            for (int i = 0; i < glassAmount; i++)
            {
                Glass newGlass = new Glass();
                glassShelf.Add(newGlass);
            }

            seats = new BlockingCollection<Seat>();
            for (int i = 0; i < seatAmount; i++)
            {
                Seat newSeat = new Seat();
                seats.Add(newSeat);
            }
            availableSeats = seatAmount;
            LabelMessage(guestAmountLabel, $"Guests: 0");
            LabelMessage(glassesAmountLabel, $"Available glasses: {glassShelf.Count}" +
                                             $"\nTotal: {glassAmount}");
            LabelMessage(availableSeatsAmountLabel, $"Available seats: {availableSeats}" +
                                             $"\nTotal: {seatAmount}");

            guests = new BlockingCollection<Guest>();
            dirtyGlasses = new BlockingCollection<Glass>();
            guestsWaitingForBeer = new ConcurrentQueue<Guest>();
            guestsWaitingForSeat = new ConcurrentQueue<Guest>();

            Bouncer bouncer = new Bouncer(this);
            bouncer.LetGuestsIn();
            Bartender bartender = new Bartender(this);
            bartender.Start();
            Waiter waiter = new Waiter(this);
            waiter.Start();

            Task.Run(() =>
            {
                while (timeTillBarCloses > 0)
                {
                    UpdatePubTimer();
                    Thread.Sleep(1000 / simulationSpeed);
                }
            });

            Task.Run(() =>
            {
                while (simulationInitiated)
                {
                    if (timeTillBarCloses <= 0 && AreTasksComplete())
                    {
                        simulationInitiated = false;
                        Dispatcher.Invoke(() =>
                        {
                            openCloseBarButton.IsEnabled = true;
                            openCloseBarButton.Content = "Open bar";
                        });
                    }
                    Thread.Sleep(250 / simulationSpeed);
                }
            });
        }

        private bool AreTasksComplete()
        {
            bool tasksCompleted = true;
            foreach (Task task in activeTasks)
            {
                if (!task.IsCompleted) { tasksCompleted = false; }
            }
            return tasksCompleted;
        }

        public void ListBoxMessage(ListBox listBox, string message)
        {
            double elapsedTime = SecondsBetweenDates(dateTimeStart, DateTime.Now);
            elapsedTime = Math.Round(elapsedTime, 1, MidpointRounding.AwayFromZero);
            Dispatcher.Invoke(() =>
            {
                listBox.Items.Insert(0, $"({elapsedTime}) {message}");
                listBox.Items.Refresh();
            });
        }
        
        public void LabelMessage(Label label, string message)
        {
            Dispatcher.Invoke(() =>
            {
                label.Content = message;
            });
        }
        public void UpdatePubTimer()
        {
            double elapsedTime = SecondsBetweenDates(dateTimeLastUpdate, DateTime.Now);
            timeTillBarCloses -= elapsedTime;
            dateTimeLastUpdate = DateTime.Now;
            LabelMessage(timeTillBarClosesLabel, "Time till bar closes: " + (int)timeTillBarCloses);
        }

        public double SecondsBetweenDates(DateTime earlierTime, DateTime laterTime)
        {
            TimeSpan elapsedTime = (laterTime - earlierTime);
            double elapsedTimeSeconds = elapsedTime.TotalSeconds;
            return elapsedTimeSeconds;
        }
    }
}
