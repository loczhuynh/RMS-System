using System.Windows.Input;
using RMS.UI.View;
using RMS.UI.View.Employee;
using RMS.Client.BL;

namespace RMS.UI.ViewModel
{
    // This class is the root for all windows. All XAML namespaces are relative
    // to this class instance.
    public class VMWindowSwitch : VMBase
    {
        #region Variables
        UISelect uiChooseWindow;
        CMainWindow uiCustomer;
        static VMUsers vmUsers;
        static VMFood vmFood;
        RelayCommand openCustomer;
        RelayCommand openEmployee;
        #endregion // Variables

        #region Constructors
        public VMWindowSwitch(string displayName) : base(displayName) { }
        #endregion // Constructors

        #region Commands
        public ICommand OpenCustomer
        {
            get
            {
                if(openCustomer == null) { openCustomer = new RelayCommand(OpenCustomerExecute); }
                return openCustomer;
            }
        }

        public ICommand OpenEmployee
        {
            get
            {
                if (openEmployee == null) { openEmployee = new RelayCommand(OpenEmployeeExecute); }
                return openEmployee;
            }
        }

        #endregion // Commands

        #region Methods
        public bool OpenUISelect()
        {
            try
            {
                uiChooseWindow = new UISelect();
                uiChooseWindow.InitializeComponent();
                uiChooseWindow.DataContext = this;
                uiChooseWindow.Show();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void OpenCustomerExecute()
        {
            uiCustomer = new CMainWindow();
            vmUsers = new VMUsers("VMUsers");
            vmFood = new VMFood("VMFood");

            uiCustomer.InitializeComponent();

            // DataContext
            uiCustomer.staticSideBar.DataContext = vmUsers;
            uiCustomer.refillMenu.DataContext = vmFood;
            uiCustomer.menuUIGrid.DataContext = vmFood;

            // Display
            uiCustomer.Show();
        }

        private void OpenEmployeeExecute()
        {
            EMainWindow emp = new EMainWindow();

            //uiChooseWindow.Close();
            emp.InitializeComponent();
            emp.Show();
        }
        #endregion // Methods
    }
}
