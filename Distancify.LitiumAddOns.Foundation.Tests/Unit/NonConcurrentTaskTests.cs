﻿using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Distancify.LitiumAddOns.Foundation.Tests.Unit
{
    public class NonConcurrentTaskTests
    {
        private class TaskMock : NonConcurrentTask
        {
            private int _numberOfCalls = 0;
            public int SleepTime { get; set; }

            protected override void Run()
            {
                _numberOfCalls += 1;

                if (SleepTime > 0)
                {
                    Thread.Sleep(SleepTime);
                }
            }

            public int NumberOfCalls => _numberOfCalls;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Run_NoPreviousInstance_RunAsExpected()
        {
            var sut = new TaskMock();

            Task.Run(() => sut.ExecuteTask(null, null)).Wait();
            Task.Run(() => sut.ExecuteTask(null, null)).Wait();

            Assert.Equal(2, sut.NumberOfCalls);
        }



        [Fact]
        [Trait("Category", "Unit")]
        public void Run_PreviousInstanceRunning_RunOnlyOnce()
        {
            var sut = new TaskMock();

            sut.SleepTime = 1000;
            var tasks = new Task[] {
                Task.Run(() => sut.ExecuteTask(null, null)),
                Task.Run(() => sut.ExecuteTask(null, null))
            };
            Task.WaitAll(tasks);

            Assert.Equal(1, sut.NumberOfCalls);
        }

    }
}
