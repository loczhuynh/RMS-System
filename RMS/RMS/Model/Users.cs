namespace RMS.UI.Model
{
    class Users
    {
        #region Variables
        readonly string userName;
        readonly string phoneNumber;
        readonly string password;
        #endregion // Variables

        #region Properties
        public string UserName
        {
            get { return userName; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
        }

        public string Password
        {
            get { return password; }
        }
        #endregion // Properties

        #region Constructor
        public Users(string newUserName, string newPhoneNumber, string newPassword)
        {
            userName = newUserName;
            phoneNumber = newPhoneNumber;
            password = newPassword;
        }
        #endregion // Constructor
    }
}
