using System;
using System.Windows.Input;

namespace ArcFMSilverlight
{
    public class DelegateCommand : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _method;

        #region Constructors

        public DelegateCommand(Action<object> method)
            : this(method, null)
        {
        }

        public DelegateCommand(Action<object> method, Predicate<object> canExecute)
        {
            this._method = method;
            this._canExecute = canExecute;
        }

        #endregion Constructors

        #region ICommand Members

        public event EventHandler CanExecuteChanged;
        
        public bool CanExecute(object parameter)
        {
            return ((this._canExecute == null) || this._canExecute(parameter));
        }

        public void Execute(object parameter)
        {
            this._method(parameter);
        }

        #endregion ICommand Members

        #region Public Methods

        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged(EventArgs.Empty);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            EventHandler canExecuteChanged = this.CanExecuteChanged;
            if (canExecuteChanged != null)
            {
                canExecuteChanged(this, e);
            }
        }

        #endregion Protected Methods
    }

}
