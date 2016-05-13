using System;
using System.Diagnostics;
using System.Windows;
using RMS.Client.BL;
using RMS.UI.ViewModel;

namespace RMS.UI
{
    //generate proxy command
    //svcutil.exe /language:cs /out:generatedProxy.cs /config:app.config http://localhost:8000/RMS.Server.ServiceModel.Service.BL
    //end
    class ProgramStart
    {
        private static CustomerController _customerController = null;

        // Only one instance of each controller for the system
        internal static CustomerController CustomerController 
        {
            get { return _customerController; }            
        }

        private static EmployeeController _employeeController = null;
        internal static EmployeeController EmployeeController
        {
            get { return _employeeController; }
        }

        #region Methods
        [STAThread()]
        [System.Diagnostics.DebuggerNonUserCode()]
        [System.CodeDom.Compiler.GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main() {

            //Initialize
            if (_customerController == null)
                _customerController = new CustomerController();
            if (_employeeController == null)
                _employeeController = new EmployeeController();
            VMWindowSwitch startWindow = new VMWindowSwitch("StartWindow");

            // Start Application
            App app = new App();
            app.InitializeComponent();

            // Start Window
            if (StartupCheck())
            {
                if (!startWindow.OpenUISelect())
                {
                    throw new InvalidOperationException("Error: uiChooseWindow didn't initialize correctly.");
                }
                app.Run();
            }

        }

        static bool StartupCheck()
        {

            Process[] serverExists;

            // Try and find the server
            serverExists = Process.GetProcessesByName("RMS.Server");
            if(serverExists.GetLength(0) == 0)
            {
                serverExists = Process.GetProcessesByName("RMS.Server.vshost.exe");
            }

            try
            {
                if(serverExists.GetLength(0) == 0)
                    throw new InvalidOperationException("Error: The RMS.Server executable needs to be running first.");
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }

            return true;
        }
        #endregion // Methods
    }
}
