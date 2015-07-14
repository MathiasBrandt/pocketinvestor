namespace Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;

    using Microsoft.Phone.Controls;

    /// <summary>
    /// A graphical login interface displayed if a session expires.
    /// </summary>
    public partial class ReloginScreen : PhoneApplicationPage
    {
        #region Constants and Fields

        private static ReloginScreen _instance;

        #endregion

        #region Constructors and Destructors

        public ReloginScreen()
        {
            _instance = this;
            InitializeComponent();

            input_username.Text = Communication.Instance.Username;
            login.IsEnabled = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the current relogin screen.
        /// </summary>
        public static ReloginScreen Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays an error message at the screen.
        /// </summary>
        /// <param name="message">The error message.</param>
        public void Error(String message)
        {
            Contract.Requires(!ReferenceEquals(message, null));

            Dispatcher.BeginInvoke(() => text_errorMessage.Text = "ERROR:\r" + message);
            Dispatcher.BeginInvoke(() => login.IsEnabled = true);
        }

        /// <summary>
        /// Navigates back to the investment screen.
        /// </summary>
        public void ReloginComplete()
        {
            Dispatcher.BeginInvoke(() => NavigationService.GoBack());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to relogin to the server.
        /// </summary>
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            Contract.Requires(!ReferenceEquals(input_username.Text, null));
            Contract.Requires(!ReferenceEquals(input_password.Password, null));


            if (input_password.Password.Equals(String.Empty))
            {
                text_errorMessage.Text = "Please enter a password.";
            }
            else
            {
                Communication.Instance.Relogin(input_password.Password);
                login.IsEnabled = false;
            }
        }

        #endregion
    }
}