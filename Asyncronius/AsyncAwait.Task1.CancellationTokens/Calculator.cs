using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait.Task1.CancellationTokens
{
    static class Calculator
    {
        public static Task<long> Calculate(int n)
        {
            var token = CancellationToken.None;
            return Calculate(n, token);
        }

        public static Task<long> Calculate(int n, CancellationToken token)
        {
            long MakeCalculation(int input, CancellationToken cancToken)
            {
                long sum = 0;

                for (int i = 0; i < input; i++)
                {
                    if (cancToken.IsCancellationRequested)
                    {
                        throw new InvalidOperationException("The task was cancelled");
                    }

                    sum = sum + (i + 1);
                    Thread.Sleep(10);
                }

                return sum;
            }

            return Task.Factory.StartNew(() => MakeCalculation(n, token));
        }
    }
}
