/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private static readonly object _lock = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var sharedList = new List<int>();

            for (int i = 1; i <= 10; i++)
            {
                AddItemInCollection(i, sharedList).GetAwaiter().GetResult();
                PrintItem(sharedList).GetAwaiter().GetResult();
            }

            Console.ReadLine();
        }

        private static Task AddItemInCollection(int addingValue, List<int> sharedList)
        {
            lock (_lock)
            {
                var task = new Task(() =>
                {
                    sharedList.Add(addingValue);
                });

                task.Start();

                return task;
            }
        }

        private static Task PrintItem(List<int> sharedList)
        {
            lock (_lock)
            {
                var task = new Task(() =>
                {
                    var result = string.Join(", ", sharedList);
                    Console.WriteLine($"[{result}]");
                });

                task.Start();

                return task;
            }
        }
    }
}
