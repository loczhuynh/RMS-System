using RMS.Client.BL;

namespace RMS.Client
{
    class Program
    {
        //generate proxy command
        //svcutil.exe /language:cs /out:generatedProxy.cs /config:app.config http://localhost:8000/RMS.Server.ServiceModel.Service.BL
        //end
        private static CustomerController _restaurantController = null;

        public static CustomerController CustomerController
        {
            get { return _restaurantController; }
            set { _restaurantController = value; }
        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThread()]
        [System.Diagnostics.DebuggerNonUserCode()]
        [System.CodeDom.Compiler.GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main() {

            //Create an endpoint address and an instance of the WCF Client.
            if (_restaurantController == null)
                _restaurantController = new RestaurantController();
        }
    }
}
