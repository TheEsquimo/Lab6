using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        double timeTillBarCloses;
        int glassAmount = 8;
        int chairAmount = 9;
        BlockingCollection<Glass> glassShelf;
        BlockingCollection<Glass> dirtyGlasses;
        ConcurrentQueue<Guest> guestsWaitingForBeer;
        ConcurrentQueue<Guest> guestsWaitingForSeat;

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

            dirtyGlasses = new BlockingCollection<Glass>();
            guestsWaitingForBeer = new ConcurrentQueue<Guest>();
            guestsWaitingForSeat = new ConcurrentQueue<Guest>();
        }
    }
}
