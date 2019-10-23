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
        internal double timeTillBarCloses;
        int glassAmount = 8;
        int chairAmount = 9;
        internal int simulationSpeed = 1;
        internal BlockingCollection<Glass> glassShelf;
        internal BlockingCollection<Glass> dirtyGlasses;
        internal BlockingCollection<Chair> chairs;
        internal BlockingCollection<Guest> guests;
        internal ConcurrentQueue<Guest> guestsWaitingForBeer;
        internal ConcurrentQueue<Guest> guestsWaitingForSeat;
        Bartender bartender = new Bartender();
        Waiter waiter = new Waiter();
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


            /*
            Task.Run(() =>
            {
                while (true)
                {
                    int randomNameNumber = random.Next(guestNames.Count);
                    string nameOfNewGuest = guestNames[randomNameNumber];
                    Guest newGuest = new Guest(nameOfNewGuest);
                    Dispatcher.Invoke(() =>
                    {
                       guestListBox.Items.Insert(0, newGuest);
                    });
                    Thread.Sleep(1000);
                }
            });
            */


            glassShelf = new BlockingCollection<Glass>();
            for (int i = 0; i < glassAmount; i++)
            {
                Glass newGlass = new Glass();
                glassShelf.Add(newGlass);
            }
            glassShelf.CompleteAdding();

            chairs = new BlockingCollection<Chair>();
            for (int i = 0; i < chairAmount; i++)
            {
                Chair newChair = new Chair();
                chairs.Add(newChair);
            }
            chairs.CompleteAdding();

            guests = new BlockingCollection<Guest>();
            dirtyGlasses = new BlockingCollection<Glass>();
            guestsWaitingForBeer = new ConcurrentQueue<Guest>();
            guestsWaitingForSeat = new ConcurrentQueue<Guest>();
            timeTillBarCloses = 120;
            Bouncer bouncer = new Bouncer(this);
            bouncer.LetGuestsIn();

            bartender.TheMainWindow = this;
            //bartender.Start();
        }
    }
}
