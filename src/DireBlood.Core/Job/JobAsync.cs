using System;
using System.Threading.Tasks;

namespace DireBlood.Core.Job
{
    public class JobAsync<T> where T : class, new()
    {
        private readonly T _instance = new T();
        private readonly Progress<T> _progress = new Progress<T>();
        private readonly Func<IProgress<T>, T, Task> _func;

        public JobAsync(Func<IProgress<T>, T, Task> func)
        {
            _func = func;
        }

        private Action<T> _onProgressChanged;
        private Action<Exception> _onException;
        private Action _onBeforeExecution;
        private Action<T> _onSuccess;

        public JobAsync<T> OnProgressChanged(Action<T> action)
        {
            _onProgressChanged = action;
            return this;
        }

        public JobAsync<T> OnException(Action<Exception> action)
        {
            _onException = action;
            return this;
        }

        public JobAsync<T> OnBeforeExecute(Action action)
        {
            _onBeforeExecution = action;
            return this;
        }

        public JobAsync<T> OnSuccess(Action<T> action)
        {
            _onSuccess = action;
            return this;
        }


        public async Task ExecuteAsync()
        {
            _progress.ProgressChanged += HandleProgressChanged;
            try
            {
                if (_onBeforeExecution != null)
                    _onBeforeExecution();

                await _func(_progress, _instance);

                if (_onSuccess != null)
                    _onSuccess(_instance);
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
            if (_onProgressChanged != null)
                _onProgressChanged(args);
        }
    }
}