using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task1._100Tasks
{
    class Program
    {
        const int TaskAmount = 100;
        const int MaxIterationsCount = 1000;

        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. Multi threading V1.");
            Console.WriteLine("1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are not finished.");
            Console.WriteLine("Each Task should iterate from 1 to 1000 and print into the console the following string:");
            Console.WriteLine("“Task #0 – {iteration number}”.");
            Console.WriteLine();
            
            HundredTasks();

            Console.ReadLine();
        }

        static void HundredTasks()
        {
            var factory = new TaskFactory();
            var taskList = new List<Task>();

            for (var taskNumber = 0; taskNumber < TaskAmount; taskNumber++)
            {
                var newTaskItem = factory.StartNew(() => GenerateArray(taskNumber));
                taskList.Add(newTaskItem);
            }

            factory.ContinueWhenAll(taskList.ToArray(), SendFinishMessage);
        }

        private static void SendFinishMessage(Task[] obj)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("All task was ended.");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
        }

        private static void GenerateArray(int taskNumber)
        {
            var numberArray = Enumerable.Range(1, MaxIterationsCount);
            var currentTaskNumber = taskNumber;

            foreach (var printNumber in numberArray)
            {
                Output(currentTaskNumber, printNumber);
            }
        }

        static void Output(int taskNumber, int iterationNumber)
        {
            Console.Write($"Task #{taskNumber} – {iterationNumber}  ");
        }
    }
}
