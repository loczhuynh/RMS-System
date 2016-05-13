using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace RMS.UI.ViewModel
{
    public class VMBase : INotifyPropertyChanged
    {
        #region Variables
        public event PropertyChangedEventHandler PropertyChanged;
        private string displayName;
        #endregion

        #region Properties
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        #endregion

        #region Constructors
        public VMBase(string name) { DisplayName = name; }
        #endregion

        #region Methods
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                throw new Exception(msg);
            }
        }
        #endregion

        #region Classes
        public class RelayCommand : ICommand
        {
            #region Variables
            readonly Action executeNoVar;
            readonly Action<object> execute;
            readonly Predicate<object> canExecute;
            #endregion

            #region Constructors
            // Method without a parameter. Predicate needs a parameter type so accepting
            // an object here. Can be any type.
            public RelayCommand(Action execute) : this(execute, null) { }
            public RelayCommand(Action execute, Predicate<object> canExecute)
            {
                if (execute == null)
                {
                    throw new ArgumentNullException("method passed to RelayCommand is null");
                }
                executeNoVar = execute;
                this.canExecute = canExecute;
            }

            // Method with a parameter
            public RelayCommand(Action<object> execute) : this(execute, null) { }
            public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            {
                if (execute == null)
                    throw new ArgumentNullException("Method passed to RelayCommand is null");
                this.execute = execute;
                this.canExecute = canExecute;
            }
            #endregion

            #region Methods
            [DebuggerStepThrough]
            public bool CanExecute(object parameter)
            {
                return canExecute == null ? true : canExecute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                if (parameter != null) { execute(parameter); }
                else { executeNoVar(); }
            }
            #endregion
        }
        #endregion
    }
}
