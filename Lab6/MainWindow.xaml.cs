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
            LabelMessage(guestAmountLabel, $"Guests: 0");
            LabelMessage(glassesAmountLabel, $"Available glasses: {pub.glassShelf.Count}" +
                                             $"\nTotal: {pub.glassAmount}");
            LabelMessage(availableSeatsAmountLabel, $"Available seats: {pub.availableSeats}" +
                                             $"\nTotal: {pub.seatAmount}");

            Bouncer bouncer = new Bouncer(this, pub, ListBoxMessage, LabelMessage);
            bouncer.Work();
            Bartender bartender = new Bartender(this, pub, ListBoxMessage);
            bartender.Start();
            Waiter waiter = new Waiter(this, pub, ListBoxMessage);
            waiter.Start();

            Task.Run(() =>
            {
                while (pub.timeTillBarCloses > 0)
                {
                    UpdatePubTimer();
                    Thread.Sleep(1000 / pub.simulationSpeed);
                }
            });

            Task.Run(() =>
            {
                while (pub.simulationInitiated)
                {
                    if (pub.timeTillBarCloses <= 0 && AreTasksComplete())
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

        private bool AreTasksComplete()
        {
            bool tasksCompleted = true;
            foreach (Task task in pub.activeTasks)
            {
                if (!task.IsCompleted) { tasksCompleted = false; }
            }
            return tasksCompleted;
        }
        
        private void SimulationSpeedValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pub.simulationSpeed = (int)simulationSpeedSlider.Value;
        }

        public void ListBoxMessage(object sender, string message)
        {
            double elapsedTime = SecondsBetweenDates(pub.dateTimeStart, DateTime.Now);
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
            double elapsedTime = SecondsBetweenDates(pub.dateTimeLastUpdate, DateTime.Now);
            pub.timeTillBarCloses -= elapsedTime * pub.simulationSpeed;
            pub.dateTimeLastUpdate = DateTime.Now;
            LabelMessage(timeTillBarClosesLabel, "Time till bar closes: " + (int)pub.timeTillBarCloses);
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
