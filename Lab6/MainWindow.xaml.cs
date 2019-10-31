using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        Pub pub;
        public MainWindow()
        {
            InitializeComponent();
            openCloseBarButton.Click += OnOpenCloseBarButtonClicked;
            simulationSpeedSlider.ValueChanged += SimulationSpeedValueChanged;
            pub = new Pub(this, ListBoxMessage, LabelMessage);
        }

        private void OnOpenCloseBarButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!pub.simulationInitiated)
            {
                UISettingsOnBarOpen();
                InitiateSimulation();
            }
            else
            {
                openCloseBarButton.IsEnabled = false;
                openCloseBarButton.Content = "Finishing up for the day";
                pub.timeTillBarCloses = 0;
            }
        }

        public void InitiateSimulation()
        {
            pub = new Pub(this, ListBoxMessage, LabelMessage);
            pub.simulationInitiated = true;
            LabelMessage(Labell.GuestAmount, $"Guests: 0");
            LabelMessage(Labell.GlassesAvailable, $"Available glasses: {pub.glassShelf.Count}" +
                                             $"\nTotal: {pub.glassAmount}");
            LabelMessage(Labell.SeatsAvailable, $"Available seats: {pub.availableSeats}" +
                                             $"\nTotal: {pub.seatAmount}");

            Bouncer bouncer = new Bouncer(pub, ListBoxMessage);
            bouncer.Work();
            Bartender bartender = new Bartender(pub, ListBoxMessage, LabelMessage);
            bartender.Start();
            Waiter waiter = new Waiter(pub, ListBoxMessage, LabelMessage);
            waiter.Start();

            Task.Run(() =>
            {
                while (pub.timeTillBarCloses > 0)
                {
                    pub.UpdatePubTimer();
                    Thread.Sleep(1000 / pub.simulationSpeed);
                }
            });

            Task.Run(() =>
            {
                while (pub.simulationInitiated)
                {
                    if (pub.timeTillBarCloses <= 0 && pub.AreTasksComplete())
                    {
                        pub.simulationInitiated = false;
                        Dispatcher.Invoke(() =>
                        {
                            UISettingsOnBarClose();
                        });
                    }
                    Thread.Sleep(250 / pub.simulationSpeed);
                }
            });
        }

        public void SimulationSpeedValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pub.simulationSpeed = (int)simulationSpeedSlider.Value;
        }

        public void ListBoxMessage(object sender, string message)
        {
            double elapsedTime = pub.SecondsBetweenDates(pub.dateTimeStart, DateTime.Now);
            elapsedTime = Math.Round(elapsedTime, 1, MidpointRounding.AwayFromZero);
            Dispatcher.Invoke(() =>
            {
                ListBox listBox = null;
                if (sender is Bartender) { listBox = bartenderListBox; }
                else if (sender is Bouncer || sender is Guest) { listBox = guestListBox; }
                else if (sender is Waiter) { listBox = waiterListBox; }
                if (listBox != null)
                {
                    listBox.Items.Insert(0, $"({elapsedTime}) {message}");
                    listBox.Items.Refresh();
                }
            });
        }

        public void LabelMessage(Labell editLabel, string message)
        {
            double elapsedTime = pub.SecondsBetweenDates(pub.dateTimeStart, DateTime.Now);
            elapsedTime = Math.Round(elapsedTime, 1, MidpointRounding.AwayFromZero);
            Dispatcher.Invoke(() =>
            {
                Label label = null;
                if (editLabel == Labell.GuestAmount) { label = guestAmountLabel; }
                else if (editLabel == Labell.SeatsAvailable) { label = availableSeatsAmountLabel; }
                else if (editLabel == Labell.GlassesAvailable)  { label = glassesAmountLabel; }
                else if (editLabel == Labell.TimeLeft) { label = timeTillBarClosesLabel; }
                if (label != null) { label.Content = message; }
            });
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
