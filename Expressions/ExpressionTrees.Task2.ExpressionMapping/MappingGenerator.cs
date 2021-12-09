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
            var mapFunction = GenerateMapperFunc<TSource, TDestination>();

            return new Mapper<TSource, TDestination>(mapFunction.Compile());
        }

        private Expression<Func<TSource, TDestination>> GenerateMapperFunc<TSource, TDestination>()
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
                var attribute = ((MapAttribute[])sourceProperty.GetCustomAttributes(typeof(MapAttribute), false)).FirstOrDefault();
                var sourcePropertyName = attribute != null ? attribute.PropertyName : sourceProperty.Name;
                var sourcePropertyType = attribute != null ? attribute.PropertyType : sourceProperty.PropertyType;

                if (targetProperties.TryGetValue(sourcePropertyName, out PropertyInfo targetProperty))
                {
                    Expression temporaryPropertyAssign = null;
                    if (sourceProperty.PropertyType != sourcePropertyType)
                    {

                        if (sourcePropertyType.CanChangeType(sourcePropertyType))
                        {
                            temporaryPropertyAssign = Expression.Convert(Expression.Property(source, sourceProperty), targetProperty.PropertyType);
                        }
                        else
                        {
                            temporaryPropertyAssign = Expression.Default(targetProperty.PropertyType);
                        }

                        if (sourcePropertyType == typeof(string))
                        {
                            var property = Expression.Property(source, sourceProperty);

                            temporaryPropertyAssign = Expression.Call(Expression.Convert(property, typeof(object)),
                                                                    typeof(object).GetMethod("ToString"));
                        }
                    }


                    Expression sourcePropertyAssign;

                    if (sourcePropertyType != targetProperty.PropertyType)
                    {
                        if (temporaryPropertyAssign is null)
                        {
                            if (sourcePropertyType.CanChangeType(targetProperty.PropertyType))
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
                            if (sourcePropertyType.CanChangeType(targetProperty.PropertyType))
                            {
                                sourcePropertyAssign = Expression.Convert(temporaryPropertyAssign, targetProperty.PropertyType);
                            }
                            else
                            {
                                sourcePropertyAssign = Expression.Default(targetProperty.PropertyType);
                            }
                        }
                    }
                    else
                    {
                        if (temporaryPropertyAssign is null)
                        {
                            sourcePropertyAssign = Expression.Property(source, sourceProperty);
                        }
                        else
                        {
                            sourcePropertyAssign = temporaryPropertyAssign;
                        }
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
