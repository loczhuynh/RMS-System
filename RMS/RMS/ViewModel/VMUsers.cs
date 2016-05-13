using System.Windows.Input;
using RMS.UI.Model;
using System.Diagnostics;

namespace RMS.UI.ViewModel
{
    public class VMUsers : VMBase
    {
        #region Variables
        private Users testUser;
        private RelayCommand checkLogin;
        private string currentUser;
        private string uiPhoneNumber;
        private string uiPassword;
        private string uiButtonState;
        private int loginStatus;
        public enum loginTypes { loggedIn, loggedOut, badLogin };
        #endregion // Variables

        #region Constructors
        // Hint: Users(userName, phoneNumber, password)
        public VMUsers(string propName) : base(propName)
        {
            // Set users
            testUser = new Users("Matt", "1112223333", "password");
            // Set properties
            UiPhoneNumber = "Enter your phone number";
            UiPassword = "Enter your password";
            UiButtonState = "Login";
            LoginStatus = (int)loginTypes.loggedOut;
            CurrentUser = string.Empty;
        }
        #endregion // Constructors

        #region Properties
        public string UiPhoneNumber
        {
            get { return uiPhoneNumber; }
            set
            {
                uiPhoneNumber = value;
                RaisePropertyChanged("UiPhoneNumber");
            }
        }

        public string UiPassword
        {
            get { return uiPassword; }
            set
            {
                uiPassword = value;
                RaisePropertyChanged("UiPassword");
            }
        }

        public string UiButtonState
        {
            get { return uiButtonState; }
            set
            {
                uiButtonState = value;
                RaisePropertyChanged("UiButtonState");
            }
        }

        public string CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                RaisePropertyChanged("CurrentUser");
            }
        }

        public int LoginStatus
        {
            get { return loginStatus; }
            set
            {
                loginStatus = value;
                RaisePropertyChanged("LoginStatus");
            }
        }
        #endregion // Properties

        #region Commands
        public ICommand CheckLogin
        {
            get
            {
                if (checkLogin == null) { checkLogin = new RelayCommand(CheckLoginExecute); }
                return checkLogin;
            }
        }

        public void CheckLoginExecute()
        {
            string user;

            user = GetUser();
            UpdateLoginStatus(user);
            UpdateUser();
        }
        #endregion // Commands

        #region Methods
        // Use credentials to retrieve user from model
        public string GetUser()
        {
            bool correctUser = false;

            if ((UiPhoneNumber == testUser.PhoneNumber) && (UiPassword == testUser.Password))
            {
                correctUser = true;
            }

            // Update login textboxes
            UiPhoneNumber = "Enter your phone number";
            UiPassword = "Enter your password";

            if (correctUser) { return testUser.UserName; }
            else { return null; }
        }

        // Change property that determines if user is logged in
        public void UpdateLoginStatus(string user)
        {
            if(user == null) { LoginStatus = (int)loginTypes.loggedOut; }
            else if(user == "!BadLogin") { LoginStatus = (int)loginTypes.badLogin; }
            else { LoginStatus = (int)loginTypes.loggedIn; }
        }

        // Change property of the user name the view sees
        public void UpdateUser()
        {
            if(LoginStatus == (int)loginTypes.loggedOut) { CurrentUser = string.Empty; }
            else if(LoginStatus == (int)loginTypes.badLogin) { CurrentUser = "!BadLogin"; }
            else { CurrentUser = testUser.UserName; }
        }
        #endregion // Methods
    }
}
