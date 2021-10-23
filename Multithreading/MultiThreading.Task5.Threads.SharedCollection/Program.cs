/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        static AutoResetEvent readHandler = new AutoResetEvent(false);
        static AutoResetEvent writeHandler = new AutoResetEvent(true);
        private static List<int> sharedList = new List<int>();

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();


            var fact = new TaskFactory();

            fact.StartNew(AddItemInCollection);
            fact.StartNew(AddItemInCollection);
            fact.StartNew(PrintCollection);

            Console.ReadLine();
        }

        private static void AddItemInCollection()
        {
            for (int i = 1; i <= 10; i++)
            {
                writeHandler.WaitOne();

                sharedList.Add(i);

                readHandler.Set();
            }
        }

        private static void PrintCollection()
        {
            while(sharedList.Count < 10)
            {
                readHandler.WaitOne();

                var result = string.Join(", ", sharedList);
                Console.WriteLine($"[{result}]");

                writeHandler.Set();
            }
        }
    }
}
