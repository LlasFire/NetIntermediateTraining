/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            Console.WriteLine("---------------------Variant A---------------------");
            Console.WriteLine();

            SettingsForRegardlessResult();
            Thread.Sleep(2000);

            Console.WriteLine("---------------------Variant B---------------------");
            Console.WriteLine();

            SettingsForOnlyFaultResult();
            Thread.Sleep(2000);

            Console.WriteLine("---------------------Variant C---------------------");
            Console.WriteLine();

            SettingsForFailedTask();
            Thread.Sleep(2000);

            Console.WriteLine("---------------------Variant D---------------------");
            Console.WriteLine();

            SettingsForCancelledTask();

            Console.ReadLine();
        }

        private static void SettingsForCancelledTask()
        {
            // Success case
            var succedTask = new Task(() =>
            {
                Console.WriteLine("Initial D task in success case");
            });
            succedTask.ContinueWith((result) =>
            {
                var failedContinueTask = new Task(() => Console.WriteLine("Continue D task in success case"));
                failedContinueTask.Start();
            }, cancellationToken: new CancellationToken(), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);

            succedTask.Start();

            // Fault case
            var cancelTokenSource = new CancellationTokenSource();
            var token = cancelTokenSource.Token;

            var faultedTask = new Task(() =>
            {
                Thread.Sleep(500);
                Console.WriteLine("Initial D task for fault case");

                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }

            }, token);

            var anotherThread = new Thread(new ThreadStart(RunContinueFalseTask));

            faultedTask.ContinueWith((result) =>
            {
                anotherThread.Start();
                Console.WriteLine($"Is current thread in threadPool? - {anotherThread.IsThreadPoolThread}");

            }, cancellationToken: new CancellationToken(),
            TaskContinuationOptions.OnlyOnRanToCompletion,
            TaskScheduler.Current);

            faultedTask.Start();

            Thread.Sleep(700);
            cancelTokenSource.Cancel();

            void RunContinueFalseTask()
            {
                var continueTask = new Task(() =>
                {
                    Console.WriteLine("Continue D task faulted");

                });
                continueTask.Start();
            }
        }

        private static void SettingsForFailedTask()
        {
            // Success case
            var succedTask = new Task(() =>
            {
                Console.WriteLine("Initial C task in success case");
            });
            succedTask.ContinueWith((result) =>
            {
                var continueTask = new Task(() => Console.WriteLine("Continue C task in success case"));
                continueTask.Start();
            }, cancellationToken: new CancellationToken(), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);

            succedTask.Start();

            // Fault case
            var faultedTask = new Task(() =>
            {
                Thread.Sleep(500);
                Console.WriteLine("Initial C task for fault case");
                
                var currentThread = Thread.CurrentThread;

                Console.WriteLine($"Current thread id #{currentThread.ManagedThreadId}");
                throw new Exception(string.Empty);
            });

            faultedTask.ContinueWith((result) =>
            {
                var continueTask = new Task(() =>
                {
                    Console.WriteLine("Continue C task faulted");
                    var currentThread = Thread.CurrentThread;

                    Console.WriteLine($"Current thread id #{currentThread.ManagedThreadId}");
                }, new CancellationToken(), TaskCreationOptions.AttachedToParent);
                continueTask.Start();
            }, cancellationToken: new CancellationToken(),
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Current);

            faultedTask.Start();
        }

        private static void SettingsForOnlyFaultResult()
        {
            // Success case
            var succedTask = new Task(() =>
            {
                Console.WriteLine("Initial B task in success case");
            });
            succedTask.ContinueWith((result) =>
            {
                var failedContinueTask = new Task(() => Console.WriteLine("Continue B task in success case"));
                failedContinueTask.Start();
            }, cancellationToken: new CancellationToken(), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);

            succedTask.Start();

            // Fault case
            var faultedTask = new Task(() =>
            {
                Thread.Sleep(500);
                Console.WriteLine("Initial B task for fault case");
                throw new Exception(string.Empty);
            });
            faultedTask.ContinueWith((result) =>
            {
                var continueTask = new Task(() => Console.WriteLine("Continue B task faulted"));
                continueTask.Start();
            }, cancellationToken: new CancellationToken(), TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);

            faultedTask.Start();
        }

        private static void SettingsForRegardlessResult()
        {
            // Success case
            var suceedTask = new Task(() => Console.WriteLine("Initial A task for succes case"));
            suceedTask.ContinueWith((result) =>
            {
                var continueTask = new Task(() => Console.WriteLine("Continue A task succeed"));
                continueTask.Start();
            });

            suceedTask.Start();

            // Fault case
            var faultedTask = new Task(() =>
            {
                Thread.Sleep(500);
                Console.WriteLine("Initial A task for fault case");
                throw new Exception(string.Empty);
            });
            faultedTask.ContinueWith((result) =>
            {
                var continueTask = new Task(() => Console.WriteLine("Continue A task faulted"));
                continueTask.Start();
            });

            faultedTask.Start();
        }
    }
}
