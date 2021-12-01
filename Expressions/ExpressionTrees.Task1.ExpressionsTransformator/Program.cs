/*
 * Create a class based on ExpressionVisitor, which makes expression tree transformation:
 * 1. converts expressions like <variable> + 1 to increment operations, <variable> - 1 - into decrement operations.
 * 2. changes parameter values in a lambda expression to constants, taking the following as transformation parameters:
 *    - source expression;
 *    - dictionary: <parameter name: value for replacement>
 * The results could be printed in console or checked via Debugger using any Visualizer.
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Expression Visitor for increment/decrement.");
            Console.WriteLine();

            var convertor = new IncDecExpressionVisitor();
            var paramsDictionary = new Dictionary<string, object>();

            #region Increment/Decrement

            var listOfExpressions = new List<Expression<Func<int, int>>>
            {
                (firstArg) => firstArg + 1,
                (firstArg) => 1 + firstArg,
                (firstArg) => firstArg + 2,

                (firstArg) => firstArg - 1,
                (firstArg) => 1 - firstArg,
                (firstArg) => firstArg - 2,
            };

            foreach (var item in listOfExpressions)
            {
                Console.WriteLine("Item before translation:");
                Console.WriteLine(item);
                Console.WriteLine();

                var translatedNode = convertor.Translate(item);

                Console.WriteLine("Item after translation:");
                Console.WriteLine(item);
                Console.WriteLine();
            }

            #endregion

            Console.ReadLine();
        }
    }
}
