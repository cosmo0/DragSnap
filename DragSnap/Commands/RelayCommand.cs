namespace DragSnap.Commands
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    /// Provides a simple relay command
    /// Copy from <see href="http://www.codeproject.com/Articles/813345/Basic-MVVM-and-ICommand-usuage-example"/>
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Whether the command can be executed
        /// </summary>
        private readonly Predicate<object> canExecute;

        /// <summary>
        /// The action to execute
        /// </summary>
        private readonly Action<object> execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class
        /// </summary>
        /// <param name="execute">The action to execute</param>
        public RelayCommand(Action<object> execute) : this(execute, DefaultCanExecute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class
        /// </summary>
        /// <param name="execute">The action to execute</param>
        /// <param name="canExecute">Whether the action can be executed</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Gets or sets the handler to check if the action can be executed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the action can be executed
        /// </summary>
        /// <param name="parameter">The action parameter</param>
        /// <returns>Whether the action can be executed</returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return this.canExecute != null && this.canExecute(parameter);
        }

        /// <summary>
        /// Executes the action
        /// </summary>
        /// <param name="parameter">The action parameter</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        /// <summary>
        /// Provides a default handler for the "can execute" handler
        /// </summary>
        /// <param name="parameter">The action parameter</param>
        /// <returns>Returns always true</returns>
        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
    }
}