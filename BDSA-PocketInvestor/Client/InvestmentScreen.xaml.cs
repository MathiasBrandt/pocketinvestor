
namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using Microsoft.Phone.Controls;
    using Microsoft.Silverlight.Testing;

    /// <summary>
    /// A graphical representation of all stock groups and private user investment data.
    /// </summary>
    public partial class InvestmentScreen : PhoneApplicationPage
    {
        #region Constants and Fields

        private readonly List<StockGroupComponent> _stockGroupComponents = new List<StockGroupComponent>();

        private static InvestmentScreen _instance;

        private Boolean _dataIsSyncedWithServer;

        #endregion

        #region Constructors and Destructors

        public InvestmentScreen()
        {
            _instance = this;
            InitializeComponent();

            Communication.Instance.StockGroups.ForEach(
                sg => _stockGroupComponents.Add(new StockGroupComponent(this, sg)));

            text_sumOfAllSliders.Text = "Total investment percentage: " + SumOfAllSliders + "%";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the current investment screen.
        /// </summary>
        public static InvestmentScreen Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Sets a value indicating if the date has been submitted to the server.
        /// </summary>
        public Boolean DataHasBeenSubmitted
        {
            set
            {
                _dataIsSyncedWithServer = value;
            }
        }

        /// <summary>
        /// The sum of the values of all the sliders.
        /// </summary>
        public int SumOfAllSliders
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == _stockGroupComponents.Sum(sgc => sgc.SliderValue));

                return _stockGroupComponents.Sum(sgc => sgc.SliderValue);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a dictinary of stock group names and their associated investment percentage.
        /// </summary>
        private Dictionary<String, int> Investments
        {
            get
            {
                Dictionary<String, int> investments = new Dictionary<string, int>();
                _stockGroupComponents.ForEach(sgc => investments.Add(sgc.StockGroupName, sgc.SliderValue));
                return investments;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays a message box with a warning about a connection error.
        /// </summary>
        public void ConnectionError()
        {
            MessageBoxResult mbr = MessageBox.Show("Please make sure your phone is connected to the internet and try again.", "Connection Error", MessageBoxButton.OK);
        }

        /// <summary>
        /// Displays a message box with info about session expiration, navigates to the relogin screen.
        /// </summary>
        public void ReLogin()
        {
            MessageBoxResult mbr = MessageBox.Show("Your session has expired. To submit your data please log in again.", "Session Expired", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri("/ReloginScreen.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// Updates the message shown in the botton of the screen.
        /// </summary>
        public void UpdateControlPanelText()
        {
            text_sumOfAllSliders.Text = "Total investment percentage: " + SumOfAllSliders + "%";

            if(!IsDataSaved())
            {
                text_dataIsSynced.Foreground = new SolidColorBrush(Colors.Orange);
                text_dataIsSynced.Text = "Changes have not been submitted.";
            }
            else
            {
                text_dataIsSynced.Text = "";
            }
        }

        /// <summary>
        /// Enables or disables the submit button based on IsDataSaved()
        /// </summary>
        public void UpdateSubmitButtonStatus()
        {
            submitButton.IsEnabled = (!IsDataSaved() || !_dataIsSyncedWithServer);
        }

        /// <summary>
        /// Updates the message shown in the button of the screen.
        /// </summary>
        public void UpdateSynchronizedText()
        {
            Dispatcher.BeginInvoke(
                delegate()
                    {
                        UpdateSubmitButtonStatus();
                        if (_dataIsSyncedWithServer)
                        {
                            text_dataIsSynced.Foreground = new SolidColorBrush(Colors.Green);
                            text_dataIsSynced.Text = "Changes were successfully submitted to the server.";
                        }
                        else
                        {
                            text_dataIsSynced.Foreground = new SolidColorBrush(Colors.Red);
                            text_dataIsSynced.Text = "Changes have not been submitted to the server.";
                        }
                    });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Ask the user if she/he wants to log out, and goes back to the login screen.
        /// </summary>
        private void Logout()
        {
            MessageBoxResult mbr = MessageBox.Show("Are you sure you want to log out?\rWARNING: Changes since last submit will not be saved!", "Confirm log out", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.OK)
            {
                Communication.Instance.CloseConnection();
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Back button event - maps to Logout().
        /// </summary>
        private void BackButtonClick(object sender, CancelEventArgs args)
        {
            args.Cancel = true;
            Logout();
        }

        /// <summary>
        /// Verifies wether the displayed data matches the saved data in the communication instance.
        /// </summary>
        /// <returns>True if data matches.</returns>
        private Boolean IsDataSaved()
        {
            foreach (StockGroupComponent stockGroupComponent in _stockGroupComponents)
            {
                if (stockGroupComponent.SliderValue != Communication.Instance.Investments(stockGroupComponent.StockGroupName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Logout button event - maps to Logout().
        /// </summary>
        private void LogoutButtonClick(object sender, RoutedEventArgs args)
        {
            Logout();
        }

        /// <summary>
        /// Reset button event - maps to ResetInvestmentValues().
        /// </summary>
        private void ResetButtonClick(object sender, RoutedEventArgs args)
        {
            ResetInvestmentValues();
        }

        /// <summary>
        /// Reverts the displayed investment values to the saved values in the communication instance.
        /// </summary>
        private void ResetInvestmentValues()
        {
            Contract.Ensures(Contract.ForAll(_stockGroupComponents, stock => stock.SliderValue == Communication.Instance.Investments(stock.StockGroupName)));

            foreach (StockGroupComponent stockGroupComponent in _stockGroupComponents)
            {
                stockGroupComponent.SliderValue = Communication.Instance.Investments(stockGroupComponent.StockGroupName);
            }
        }

        /// <summary>
        /// Submit button event - maps to SubmitInvestmentValues().
        /// </summary>
        private void SubmitButtonClick(object sender, RoutedEventArgs args)
        {
            SubmitInvestmentValues();
        }

        /// <summary>
        /// Saves the displayed investments values in the communication instance.
        /// </summary>
        private void SubmitInvestmentValues()
        {
            Contract.Ensures(Contract.ForAll(_stockGroupComponents, stock => stock.SliderValue == Communication.Instance.Investments(stock.StockGroupName)));

            Boolean submit = Communication.Instance.SubmitData(Investments);

            if (!submit)
            {
                MessageBox.Show("Malformed data, please restart the application.", "Data exception", MessageBoxButton.OK);
                _dataIsSyncedWithServer = false;
                UpdateSynchronizedText();
                return;
            }

            foreach (StockGroupComponent stockGroupComponent in _stockGroupComponents)
            {
                stockGroupComponent.UpdateGroupInfoHeader();
            }

            submitButton.IsEnabled = false;

            UpdateControlPanelText();
        }

        #endregion

        private void ResetButtonHold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var testPage = UnitTestSystem.CreateTestPage() as IMobileTestPage;
            BackKeyPress += (x, xe) => xe.Cancel = testPage.NavigateBack();
            (Application.Current.RootVisual as PhoneApplicationFrame).Content = testPage;
        }
    }
}