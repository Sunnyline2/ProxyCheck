using System;

namespace DireBlood.Core.Services
{
    public class Job<T> where T : class, new()
    {
        private readonly Action<IProgress<T>, T> action;
        public readonly T EventArgs = new T();
        private readonly Progress<T> progress = new Progress<T>();
        private Action onBeforeExecution;
        private Action<Exception> onException;

        private Action<T> onProgressChanged;
        private Action<T> onSuccess;

        public Job(Action<IProgress<T>, T> action)
        {
            this.action = action;
        }

        public Job<T> OnProgressChanged(Action<T> action)
        {
            onProgressChanged = action;
            return this;
        }

        public Job<T> OnException(Action<Exception> action)
        {
            onException = action;
            return this;
        }

        public Job<T> OnBeforeExecute(Action action)
        {
            onBeforeExecution = action;
            return this;
        }

        public Job<T> OnSuccess(Action<T> action)
        {
            onSuccess = action;
            return this;
        }

        public void Execute()
        {
            progress.ProgressChanged += HandleProgressChanged;
            try
            {
                if (onBeforeExecution != null)
                    onBeforeExecution();

                action(progress, EventArgs);

                if (onSuccess != null)
                    onSuccess(EventArgs);
            }
            catch (Exception exception)
            {
                if (onException == null)
                    throw;
                onException(exception);
            }
            finally
            {
                progress.ProgressChanged -= HandleProgressChanged;
            }
        }

        private void HandleProgressChanged(object sender, T args)
        {
            if (onProgressChanged != null)
                onProgressChanged(args);
        }
    }
}