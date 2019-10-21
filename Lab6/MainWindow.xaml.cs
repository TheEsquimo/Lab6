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
        internal BlockingCollection<Glass> glassShelf;
        internal BlockingCollection<Glass> dirtyGlasses;
        internal BlockingCollection<Chair> chairs;
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

            Thread.Sleep(1);

            
            

            dirtyGlasses = new BlockingCollection<Glass>();
            guestsWaitingForBeer = new ConcurrentQueue<Guest>();
            guestsWaitingForSeat = new ConcurrentQueue<Guest>();

            timeTillBarCloses = 120;
            Bouncer bouncer = new Bouncer();
            bouncer.TheMainWindow = this;
            bouncer.LetGuestsIn();
        }
    }
}
