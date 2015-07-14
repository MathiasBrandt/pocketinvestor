
namespace Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Navigation;

    using Microsoft.Phone.Controls;
    using Microsoft.Silverlight.Testing;

    /// <summary>
    /// A graphical login interface displayed at start up.
    /// </summary>
    public partial class LoginScreen : PhoneApplicationPage
    {
        #region Constants and Fields

        private static LoginScreen _instance;

        #endregion

        #region Constructors and Destructors

        public LoginScreen()
        {
            _instance = this;
            InitializeComponent();
            login.IsEnabled = false;
            Communication.CreateNewInstance();
        }

        #endregion

        #region Public Properties
        
        /// <summary>
        /// Returns the current login screen.
        /// </summary>
        public static LoginScreen Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays an error message on the screen.
        /// </summary>
        /// <param name="message">The error message.</param>
        public void Error(String message)
        {
            Contract.Requires(!ReferenceEquals(message, null));
            Dispatcher.BeginInvoke(() => text_errorMessage.Text = "ERROR:\r" + message);
        }

        /// <summary>
        /// Navigates to the investment screen.
        /// </summary>
        public void LoginComplete()
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/InvestmentScreen.xaml", UriKind.Relative)));
        }

        /// <summary>
        /// Enables the login button on the screen.
        /// </summary>
        public void MayLogin()
        {
            Dispatcher.BeginInvoke(() => login.IsEnabled = true);
        }

        /// <summary>
        /// Sets the status message to be displayed on the screen.
        /// </summary>
        /// <param name="message">Current login status.</param>
        public void SetStatusMessage(String message)
        {
            Contract.Requires(!ReferenceEquals(message, null));
            Dispatcher.BeginInvoke(() => text_statusMessage.Text = message);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new communication instance and empties the text fields.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            login.IsEnabled = false;
            Communication.CreateNewInstance();

            text_statusMessage.Text = "";
            text_errorMessage.Text = "";
        }

        /// <summary>
        /// Verifies that the username and password fields aren't empyt and atempts to login.
        /// </summary>
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            Contract.Requires(!ReferenceEquals(input_username.Text, null));
            Contract.Requires(!ReferenceEquals(input_password.Password, null));

            text_errorMessage.Text = "";
            
            if (input_username.Text.Equals(String.Empty))
            {
                text_errorMessage.Text = "Please enter a username.";
            }
            else if (input_password.Password.Equals(String.Empty))
            {
                text_errorMessage.Text = "Please enter a password.";
            }
            else
            {
                Boolean success = Communication.Instance.Login(input_password.Password, input_username.Text);
                login.IsEnabled = !success;

                if (!success)
                {
                    text_errorMessage.Text = "Username does not exist.";
                }
            }
        }

        #endregion
    }
}