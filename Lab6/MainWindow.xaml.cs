using System;
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
        internal double timeTillBarCloses = 120;
        internal int glassAmount = 8;
        internal int seatAmount = 9;
        internal int availableSeats;
        internal int simulationSpeed = 1;
        internal BlockingCollection<Glass> glassShelf;
        internal BlockingCollection<Glass> dirtyGlasses;
        internal BlockingCollection<Seat> chairs;
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
            Random random = new Random();

            glassShelf = new BlockingCollection<Glass>();
            for (int i = 0; i < glassAmount; i++)
            {
                Glass newGlass = new Glass();
                glassShelf.Add(newGlass);
            }

            chairs = new BlockingCollection<Seat>();
            for (int i = 0; i < seatAmount; i++)
            {
                Seat newSeat = new Seat();
                chairs.Add(newSeat);
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
        }

        public void ListBoxMessage(ListBox listBox, string message)
        {
            Dispatcher.Invoke(() =>
            {
                listBox.Items.Insert(0, message);
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
    }
}
