/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            HandleRandomArrayAndCalculateAverage().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        private static async Task<int[]> GenerateArrayAsync()
        {
            int[] CreateArray()
            {
                var randomizer = new Random();
                var result =  Enumerable.Repeat(0, 10)
                                 .Select(i => randomizer.Next(1, 80))
                                 .ToArray();

                PrintArrayResults(result);
                return result;
            }

            var task = new Task<int[]>(CreateArray);
            task.Start();

            return await task;
        }

        private static async Task<int[]> MultipleArrayAsync()
        {
            int[] MultipleArray()
            {
                var sourceArray = GenerateArrayAsync().GetAwaiter().GetResult();
                var randomInteger = new Random().Next(1, 35);

                var changedArray = sourceArray.Select(item => item * randomInteger).ToArray();

                PrintArrayResults(changedArray);
                return changedArray;
            }

            var task = new Task<int[]>(MultipleArray);
            task.Start();

            return await task;
        }

        private static async Task<int[]> SortedArrayAsync()
        {
            int[] SortedArray()
            {
                var sourceArray = MultipleArrayAsync().GetAwaiter().GetResult();

                var changedArray = sourceArray.OrderBy(item => item).ToArray();

                PrintArrayResults(changedArray);
                return changedArray;
            }

            var task = new Task<int[]>(SortedArray);
            task.Start();

            return await task;
        }

        private static Task HandleRandomArrayAndCalculateAverage()
        {
            void HandleArray()
            {
                var sourceArray = SortedArrayAsync().GetAwaiter().GetResult();

                var average = sourceArray.Average();

                Console.WriteLine($"The average value for this array is {average}");
            }

            var task = new Task(HandleArray);
            task.Start();

            return task;
        }

        private static void PrintArrayResults(int[] source)
        {
            if (source != null &&
                source.Any())
            {
                var resultString = string.Join(", ", source);
                Console.WriteLine(resultString);
            }

            Console.WriteLine("------------------------------------------------");
        }
    }
}
