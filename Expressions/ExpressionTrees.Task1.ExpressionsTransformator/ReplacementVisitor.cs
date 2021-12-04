using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    public class ReplacementVisitor<T> : ExpressionVisitor
    {
        private Dictionary<string, T> _params;

        public Expression Replace(Expression exp, Dictionary<string, T> parameters)
        {
            _params = parameters;
            var translated = Visit(exp);
            return translated;
        }

        protected override Expression VisitLambda<Ti>(Expression<Ti> node)
        {
            return Expression.Lambda(Visit(node.Body), node.Parameters);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_params.ContainsKey(node.Name))
            {
                var value = _params[node.Name];
                return Expression.Constant(value);
            }

            return node;
        }
    }
}
