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

        internal bool simulationInitiated = false;
        internal bool doBusLoad;
        internal const double timeTillBarClosesStandard = 120;
        internal const int glassAmountStandard = 8;
        internal const int seatAmountStandard = 9;
        internal double timeTillBarCloses;
        internal int glassAmount;
        internal int seatAmount;
        internal int availableSeats;
        internal int simulationSpeed = 1;
        internal DateTime dateTimeStart;
        DateTime dateTimeLastUpdate;
        internal BlockingCollection<Task> activeTasks;
        internal BlockingCollection<Glass> glassShelf;
        internal BlockingCollection<Glass> dirtyGlasses;
        internal BlockingCollection<Chair> chairs;
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
            simulationSpeedSlider.ValueChanged += SimulationSpeedValueChanged;
        }

        private void OnOpenCloseBarButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!simulationInitiated)
            {
                UISettingsOnBarOpen();
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
            if (doBusLoadCheckBox.IsChecked.Value) { doBusLoad = true; }
            else { doBusLoad = false; }
            int result;
            var glassMatch = Regex.Match(glassesAmountTextBox.Text, @"^[1-9][0-9]*$");
            if (glassMatch.Success && int.TryParse(glassesAmountTextBox.Text, out result))
            {
                glassAmount = result;
            }
            else { glassAmount = glassAmountStandard; }
            var seatsMatch = Regex.Match(seatsAmountTextBox.Text, @"^[1-9][0-9]*$");
            if (seatsMatch.Success && int.TryParse(seatsAmountTextBox.Text, out result))
            {
                 seatAmount = result; 
            }
            else { seatAmount = seatAmountStandard; }
            var timeMatch = Regex.Match(timeTillBarClosesTextBox.Text, @"^[1-9][0-9]*$");
            if (timeMatch.Success && int.TryParse(timeTillBarClosesTextBox.Text, out result))
            {
                timeTillBarCloses = result;
            }
            else { timeTillBarCloses = timeTillBarClosesStandard; }
            activeTasks = new BlockingCollection<Task>();
            Random random = new Random();

            glassShelf = new BlockingCollection<Glass>();
            for (int i = 0; i < glassAmount; i++)
            {
                Glass newGlass = new Glass();
                glassShelf.Add(newGlass);
            }

            chairs = new BlockingCollection<Chair>();
            for (int i = 0; i < chairAmount; i++)
            {
                Chair newChair = new Chair();
                chairs.Add(newChair);
            }

            guests = new BlockingCollection<Guest>();
            dirtyGlasses = new BlockingCollection<Glass>();
            guestsWaitingForBeer = new ConcurrentQueue<Guest>();
            guestsWaitingForSeat = new ConcurrentQueue<Guest>();
            
            Bouncer bouncer = new Bouncer(this);
            bouncer.Work();
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
        public void UpdatePubTimer()
        {
            double elapsedTime = SecondsBetweenDates(dateTimeLastUpdate, DateTime.Now);
            timeTillBarCloses -= elapsedTime * simulationSpeed;
            dateTimeLastUpdate = DateTime.Now;
            LabelMessage(timeTillBarClosesLabel, "Time till bar closes: " + (int)timeTillBarCloses);
        }

        public double SecondsBetweenDates(DateTime earlierTime, DateTime laterTime)
        {
            TimeSpan elapsedTime = (laterTime - earlierTime);
            double elapsedTimeSeconds = elapsedTime.TotalSeconds;
            return elapsedTimeSeconds;
        }
        void UISettingsOnBarOpen()
        {
            openCloseBarButton.Content = "Close bar";
            glassesAmountTextBox.IsEnabled = false;
            seatsAmountTextBox.IsEnabled = false;
            timeTillBarClosesTextBox.IsEnabled = false;
            doBusLoadCheckBox.IsEnabled = false;
            glassesAmountTextBox.Visibility = Visibility.Hidden;
            seatsAmountTextBox.Visibility = Visibility.Hidden;
            timeTillBarClosesTextBox.Visibility = Visibility.Hidden;
            barCLoseLabel.Visibility = Visibility.Hidden;
        }
        void UISettingsOnBarClose()
        {
            openCloseBarButton.Content = "Open bar";
            openCloseBarButton.IsEnabled = true;
            glassesAmountTextBox.IsEnabled = true;
            seatsAmountTextBox.IsEnabled = true;
            timeTillBarClosesTextBox.IsEnabled = true;
            doBusLoadCheckBox.IsEnabled = true;
            glassesAmountTextBox.Visibility = Visibility.Visible;
            seatsAmountTextBox.Visibility = Visibility.Visible;
            timeTillBarClosesTextBox.Visibility = Visibility.Visible;
            barCLoseLabel.Visibility = Visibility.Visible;
        }
    }
}
