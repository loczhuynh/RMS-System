using System.Windows.Input;

namespace RMS.UI.ViewModel
{
    public class VMDynamic: VMBase
    {
        #region Variables
        private int window;
        RelayCommand switchMessageScreen;
        #endregion // Variables

        #region Constructors
        public VMDynamic(string displayName) : base(displayName) { window = 0; }
        #endregion // Constructors

        #region Properties
        public int Window { get { return window; } set { window = value; } }
        #endregion // Properties

        #region Commands
        public ICommand SwitchMessageScreen
        {
            get
            {
                if(switchMessageScreen == null) { switchMessageScreen = new RelayCommand(SwitchMessageExecute); }
                return switchMessageScreen;
            }
        }
        #endregion // Commands

        #region Methods
        public void SwitchMessageExecute()
        {
            window = (window + 1) % 3;
        }
        #endregion
    }
}
