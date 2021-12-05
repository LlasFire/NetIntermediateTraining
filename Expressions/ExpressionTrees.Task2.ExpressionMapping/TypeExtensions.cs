using System;

namespace ExpressionTrees.Task2.ExpressionMapping
{
    public static class TypeExtensions
    {
        public static bool CanChangeType(this Type fromType, Type toType)
        {
            try
            {
                var instanceOfSourceType = Activator.CreateInstance(fromType);
                Convert.ChangeType(instanceOfSourceType, toType);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
