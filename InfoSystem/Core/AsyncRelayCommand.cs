using System;
using System.Threading.Tasks;

namespace InfoSystem
{
    internal class AsyncRelayCommand : RelayCommand
    {
        public AsyncRelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
            : base(execute, canExecute)
        {
        }

        public override void Execute(object? parameter)
        {
            Task.Run(() => base.Execute(parameter));
        }
    }
}
