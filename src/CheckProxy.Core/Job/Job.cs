using System;

namespace CheckProxy.Core.Job
{
    public class Job<T> where T : class, new()
    {
        public readonly T EventArgs = new T();
        private readonly Progress<T> _progress = new Progress<T>();
        private readonly Action<IProgress<T>, T> _action;

        private Action<T> _onProgressChanged;
        private Action<Exception> _onException;
        private Action _onBeforeExecution;
        private Action<T> _onSuccess;

        public Job(Action<IProgress<T>, T> action)
        {
            _action = action;
        }

        public Job<T> OnProgressChanged(Action<T> action)
        {
            _onProgressChanged = action;
            return this;
        }

        public Job<T> OnException(Action<Exception> action)
        {
            _onException = action;
            return this;
        }

        public Job<T> OnBeforeExecute(Action action)
        {
            _onBeforeExecution = action;
            return this;
        }

        public Job<T> OnSuccess(Action<T> action)
        {
            _onSuccess = action;
            return this;
        }

        public void Execute()
        {
            _progress.ProgressChanged += HandleProgressChanged;
            try
            {
                _onBeforeExecution?.Invoke();
                _action(_progress, EventArgs);
                _onSuccess?.Invoke(EventArgs);
            }
            catch (Exception exception)
            {
                if (_onException == null)
                    throw;
                _onException(exception);
            }
            finally
            {
                _progress.ProgressChanged -= HandleProgressChanged;
            }
        }

        private void HandleProgressChanged(object sender, T args)
        {
            _onProgressChanged?.Invoke(args);
        }
    }
}