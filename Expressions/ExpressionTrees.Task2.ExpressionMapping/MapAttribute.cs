using System;

namespace ExpressionTrees.Task2.ExpressionMapping
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapAttribute : Attribute
    {
        private readonly string _propertyName;
        private readonly Type _propertyType;

        public MapAttribute(string name = null, Type type = null)
        {
            _propertyName = name;
            _propertyType = type;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public Type PropertyType
        {
            get { return _propertyType; }
        }
    }
}
