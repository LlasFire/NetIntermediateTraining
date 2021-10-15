/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static readonly Semaphore _sem = new Semaphore(1, 10);

        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");
            Console.WriteLine();

            var maxThreads = 10;

            Console.WriteLine("---------------------Variant A---------------------");
            Console.WriteLine();

            RecursiveMultiThreadingPrintingThread(maxThreads);

            Console.WriteLine("---------------------Variant B---------------------");
            Console.WriteLine();

            RecursiveMultiThreadingPrintingPool(maxThreads);

            Console.ReadLine();
        }

        private static void RecursiveMultiThreadingPrintingThread(int countOfThreads)
        {
            if (countOfThreads > 0)
            {
                HandleCount(ref countOfThreads);

                var thread = new Thread(() => RecursiveMultiThreadingPrintingThread(countOfThreads));
                thread.Start();
                thread.Join();
            }
        }

        private static void RecursiveMultiThreadingPrintingPool(object countOfThreads)
        {
            if (countOfThreads is int count && count > 0)
            {
                _sem.WaitOne();

                HandleCount(ref count);

                ThreadPool.QueueUserWorkItem(new WaitCallback(RecursiveMultiThreadingPrintingPool), count);

                _sem.Release();
            }
        }

        private static void HandleCount(ref int count)
        {
            count--;
            Console.WriteLine($"Count is {count}");
        }
    }
}
