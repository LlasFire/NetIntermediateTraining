using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{
    public class IncDecExpressionVisitor : ExpressionVisitor
    {
        public Expression Translate(Expression exp)
        {
            var translated = Visit(exp);
            return translated;
        }

        public Expression ReplaceParams(Expression exp, Dictionary<string, object> replacingDictionary)
        {
            var translated = Visit(exp);
            return translated;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch(node.NodeType)
            {
                case ExpressionType.Add:
                    return ReplaceToUnary(node);
                case ExpressionType.Subtract:
                    return ReplaceToUnary(node);
                default:
                    break;
            }

            return node;
        }

        private Expression ReplaceToUnary(BinaryExpression node)
        {
            if (!(node.Right is ConstantExpression c) || (int)c.Value != 1)
            {
                return node;
            }

            if (node.NodeType != ExpressionType.Add && node.NodeType != ExpressionType.Subtract)
            {
                return node;
            }

            if (node.Left.NodeType == ExpressionType.Parameter)
            {
                if (node.NodeType == ExpressionType.Add)
                    return Expression.Increment(node.Left);
                else
                    return Expression.Decrement(node.Left);
            }
            else if (node.Left is BinaryExpression left && (left.NodeType == ExpressionType.Add || left.NodeType == ExpressionType.Subtract))
            {
                Expression right;
                if (node.NodeType == ExpressionType.Add)
                    right = Expression.Increment(left.Right);
                else
                    right = Expression.Decrement(left.Right);

                if (left.NodeType == ExpressionType.Add)
                    return Expression.Add(Visit(left.Left), right);
                else
                    return Expression.Subtract(Visit(left.Left), right);
            }

            return node;
        }
    }
}
