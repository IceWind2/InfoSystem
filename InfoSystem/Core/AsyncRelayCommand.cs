using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InfoSystem
{
    internal class AsyncRelayCommand : ICommand
    {
        protected Func<object, Task> _execute;
        protected Func<object, bool> _canExecute;

        public AsyncRelayCommand(Func<object, Task> execute, Func<object, bool> canExecute = null)
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

        public async void Execute(object? parameter)
        {
            try
            {
                await _execute(parameter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.InnerException}", "Ошибка выполнения команды", MessageBoxButton.OK);
            }
        }
    }
}
