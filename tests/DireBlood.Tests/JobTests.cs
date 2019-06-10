using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DireBlood.Tests
{
    public class JobTests
    {
        [Fact]
        public async Task Should_Handle_Exception()
        {
            var exHandled = false;

            await new JobAsync<ProgressEventArgs>((progress, args) => throw new Exception())
                .OnException(exception => exHandled = true)
                .ExecuteAsync();

            Assert.True(exHandled);
        }

        [Fact]
        public void Should_Successfully_Complete_Job()
        {
            var isCompleted = false;
            var job = new Job<ProgressEventArgs>((progress, args) =>
            {
                args.Max = 100;
                progress.Report(args);
            }).OnSuccess(args => isCompleted = true);

            job.Execute();

            Assert.True(isCompleted, "isCompleted");
            Assert.True(job.EventArgs.Max == 100, "job.EventArgs.Max == 100");
        }

        [Fact]
        public async Task Should_Successfuly_Complete_JobAsync()
        {
            var max = 100;
            var index = 0;
            var isCompleted = false;
            var onProgressChangedCalled = false;
            var onSuccessCalled = false;
            var numberOfCalls = 0;

            var order = new Stack<string>();
            var job = new JobAsync<ProgressEventArgs>((progress, args) =>
            {
                return Task.Run(async () =>
                {
                    for (var i = 1; i <= max; i++)
                    {
                        index = i;
                        args.Current = i;
                        progress.Report(args);
                        await Task.Delay(1);
                    }

                    isCompleted = true;
                    order.Push("mainLoop");
                });
            }).OnProgressChanged(args =>
            {
                if (onProgressChangedCalled == false)
                {
                    onProgressChangedCalled = true;
                    order.Push("onProgressChanged");
                }

                numberOfCalls++;

                Assert.True(args.Current == index);
            }).OnSuccess(args =>
            {
                if (onSuccessCalled == false)
                {
                    onSuccessCalled = true;
                    order.Push("onSuccess");
                }
            });

            await job.ExecuteAsync();

            Assert.True(order.Pop() == "onSuccess");
            Assert.True(order.Pop() == "mainLoop");
            Assert.True(order.Pop() == "onProgressChanged");


            Assert.True(numberOfCalls == max);
            Assert.True(onSuccessCalled);
            Assert.True(onProgressChangedCalled);
            Assert.True(isCompleted, "isCompleted");
            Assert.True(index == max, $"index ({index}) == max ({max})");
        }

        [Fact]
        public void Should_Trigger_OnPropertyChanged()
        {
            var max = 100;
            var index = 0;
            var isCompleted = false;

            new Job<ProgressEventArgs>((progress, args) =>
                {
                    args.Max = 100;
                    progress.Report(args);

                    for (var i = 1; i <= max; i++)
                    {
                        index = i;
                        args.Current = i;
                        progress.Report(args);
                    }

                    isCompleted = true;
                }).OnProgressChanged(args => Assert.True(index == args.Current))
                .OnSuccess(args => isCompleted = true).Execute();

            Assert.True(isCompleted);
            Assert.True(max == index);
        }

        [Fact]
        public void Shouldnt_Execute_Job()
        {
            var executed = false;
            var job = new JobAsync<EventArgs>((progress, args) => Task.Run(() => { executed = true; }));
            Assert.False(executed);
        }
    }
}