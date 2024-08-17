using System;
using System.Windows;
using System.Windows.Input;

namespace InfoSystem
{
    internal class RelayCommand : ICommand
    {
        protected Action<object> _execute;
        protected Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public virtual bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public virtual void Execute(object? parameter)
        {
            try
            {
                _execute(parameter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.InnerException}", "Ошибка выполнения команды", MessageBoxButton.OK);
            }
        }
    }
}
