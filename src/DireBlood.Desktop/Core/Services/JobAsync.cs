using System;
using System.Threading.Tasks;

namespace DireBlood.Core.Services
{
    public class JobAsync<T> where T : class, new()
    {
        private readonly Func<IProgress<T>, T, Task> func;
        private readonly T instance = new T();
        private readonly Progress<T> progress = new Progress<T>();
        private Action onBeforeExecution;
        private Action<Exception> onException;
        private Action<T> onProgressChanged;
        private Action<T> onSuccess;

        public JobAsync(Func<IProgress<T>, T, Task> func)
        {
            this.func = func;
        }

        public JobAsync<T> OnProgressChanged(Action<T> action)
        {
            onProgressChanged = action;
            return this;
        }

        public JobAsync<T> OnException(Action<Exception> action)
        {
            onException = action;
            return this;
        }

        public JobAsync<T> OnBeforeExecute(Action action)
        {
            onBeforeExecution = action;
            return this;
        }

        public JobAsync<T> OnSuccess(Action<T> action)
        {
            onSuccess = action;
            return this;
        }

        public async Task ExecuteAsync()
        {
            progress.ProgressChanged += HandleProgressChanged;
            try
            {
                if (onBeforeExecution != null)
                    onBeforeExecution();

                await func(progress, instance);

                if (onSuccess != null)
                    onSuccess(instance);
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