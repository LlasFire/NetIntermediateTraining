using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task2.ExpressionMapping
{
    public class MappingGenerator
    {
        public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
        {
            var sourceParam = Expression.Parameter(typeof(TSource));
            var mapFunction = GenerateMapperFunc<TSource, TDestination>(sourceParam);

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }

        private Expression<Func<TSource, TDestination>> GenerateMapperFunc<TSource, TDestination>(ParameterExpression sourceParam)
        {
            var sourceProperties = typeof(TSource)
                                    .GetProperties()
                                    .Where(x => x.CanRead);
            var targetProperties = typeof(TDestination)
                                    .GetProperties()
                                    .Where(x => x.CanWrite)
                                    .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

            var source = Expression.Parameter(typeof(TSource), "source");
            var target = Expression.Variable(typeof(TDestination));
            var allocate = Expression.New(typeof(TDestination));
            var assignTarget = Expression.Assign(target, allocate);

            var statements = new List<Expression>
            {
                assignTarget
            };

            foreach (var sourceProperty in sourceProperties)
            {
                if (targetProperties.TryGetValue(sourceProperty.Name, out PropertyInfo targetProperty))
                {
                    Expression sourcePropertyAssign;
                    if (sourceProperty.PropertyType != targetProperty.PropertyType)
                    {
                        if (sourceProperty.PropertyType.CanChangeType(targetProperty.PropertyType))
                        {
                            sourcePropertyAssign = Expression.Convert(Expression.Property(source, sourceProperty), targetProperty.PropertyType);
                        }
                        else
                        {
                            sourcePropertyAssign = Expression.Default(targetProperty.PropertyType);
                        }
                    }
                    else
                    {
                        sourcePropertyAssign = Expression.Property(source, sourceProperty);
                    }


                    var assignProperty = Expression.Assign(
                        Expression.Property(target, targetProperty),
                        sourcePropertyAssign);

                    statements.Add(assignProperty);
                }
            }

            statements.Add(target);

            var body = Expression.Block(new[] { target }, statements);

            return Expression.Lambda<Func<TSource, TDestination>>(body, source);
        }
    }
}
