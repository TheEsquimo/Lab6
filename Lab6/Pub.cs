﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lab6
{
    public class Pub
    {
        internal bool simulationInitiated = false;
        internal bool doBusLoad;
        internal double timeTillBarClosesStandard = 120;
        internal int glassAmountStandard = 8;
        internal int seatAmountStandard = 9;
        internal double timeTillBarCloses;
        internal int glassAmount;
        internal int seatAmount;
        internal int availableSeats;
        internal int simulationSpeed = 1;
        internal DateTime dateTimeStart;
        internal DateTime dateTimeLastUpdate;
        internal BlockingCollection<Task> activeTasks;
        internal BlockingCollection<Glass> glassShelf;
        internal BlockingCollection<Glass> dirtyGlasses;
        internal BlockingCollection<Seat> seats;
        internal BlockingCollection<Guest> guests;
        internal ConcurrentQueue<Guest> guestsWaitingForBeer;
        internal ConcurrentQueue<Guest> guestsWaitingForSeat;
        private Action<Labell, string> labelMessage;
        private Action<object, string> listBoxMessage;
        private MainWindow TheMainWindow { get; }
        private Random random = new Random();
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
        public Labell EditLabel { get; }

        public Pub(MainWindow mainWindow, Action<object, string> theListBoxMessage, Action<Labell, string> theLabelMessage)
        {
            TheMainWindow = mainWindow;
            labelMessage = theLabelMessage;
            listBoxMessage = theListBoxMessage;

            if (TheMainWindow.doBusLoadCheckBox.IsChecked.Value) { doBusLoad = true; }
            else { doBusLoad = false; }
            int result;
            var glassMatch = Regex.Match(TheMainWindow.glassesAmountTextBox.Text, @"^[1-9][0-9]*$");
            if (glassMatch.Success && int.TryParse(TheMainWindow.glassesAmountTextBox.Text, out result))
            {
                glassAmount = result;
            }
            else { glassAmount = glassAmountStandard; }
            var seatsMatch = Regex.Match(TheMainWindow.seatsAmountTextBox.Text, @"^[1-9][0-9]*$");
            if (seatsMatch.Success && int.TryParse(TheMainWindow.seatsAmountTextBox.Text, out result))
            {
                seatAmount = result;
            }
            else { seatAmount = seatAmountStandard; }
            var timeMatch = Regex.Match(TheMainWindow.timeTillBarClosesTextBox.Text, @"^[1-9][0-9]*$");
            if (timeMatch.Success && int.TryParse(TheMainWindow.timeTillBarClosesTextBox.Text, out result))
            {
                timeTillBarCloses = result;
            }
            else { timeTillBarCloses = timeTillBarClosesStandard; }
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
            guests = new BlockingCollection<Guest>();
            dirtyGlasses = new BlockingCollection<Glass>();
            guestsWaitingForBeer = new ConcurrentQueue<Guest>();
            guestsWaitingForSeat = new ConcurrentQueue<Guest>();
            simulationSpeed = (int)TheMainWindow.simulationSpeedSlider.Value;
        }

        public void LetGuestsIn()
        {
            int randomNameNumber = random.Next(guestNames.Count);
            string nameOfNewGuest = guestNames[randomNameNumber];
            Guest newGuest = new Guest(nameOfNewGuest, this, TheMainWindow, listBoxMessage, labelMessage);
            guests.Add(newGuest);
            labelMessage(Labell.GuestAmount, $"Guests: {guests.Count}");
            newGuest.Start();
        }

        public void LetGuestsIn(int amountOfGuests)
        {
            for (int i = 0; i < amountOfGuests; i++)
            {
                int randomNameNumber = random.Next(guestNames.Count);
                string nameOfNewGuest = guestNames[randomNameNumber];
                Guest newGuest = new Guest(nameOfNewGuest, this, TheMainWindow, listBoxMessage, labelMessage);
                guests.Add(newGuest);
                newGuest.Start();
            }
            labelMessage(Labell.GuestAmount, $"Guests: {guests.Count}");
        }

        public bool AreTasksComplete()
        {
            bool tasksCompleted = true;
            foreach (Task task in activeTasks)
            {
                if (!task.IsCompleted) { tasksCompleted = false; }
            }
            return tasksCompleted;
        }

        public void UpdatePubTimer()
        {
            double elapsedTime = SecondsBetweenDates(dateTimeLastUpdate, DateTime.Now);
            timeTillBarCloses -= elapsedTime * simulationSpeed;
            dateTimeLastUpdate = DateTime.Now;
            labelMessage(Labell.TimeLeft, "Time till bar closes: " + (int)timeTillBarCloses);
        }

        public double SecondsBetweenDates(DateTime earlierTime, DateTime laterTime)
        {
            TimeSpan elapsedTime = (laterTime - earlierTime);
            double elapsedTimeSeconds = elapsedTime.TotalSeconds;
            return elapsedTimeSeconds;
        }
    }
}