using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Plutos.Commands
{
    class CommandAsync : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private void RaiseCanExecuteChanged()
        {
            if(CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        private bool canExecute = true;

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public async void Execute(object parameter)
        {
            canExecute = false;
            RaiseCanExecuteChanged();

            await FT();
            
            canExecute = true;
            RaiseCanExecuteChanged();

        }

        public CommandAsync(Func<Task> Func)
        {
            FT = Func;
        } 

        private Func<Task> FT;
    }
}
