using System;
using System.Linq;

namespace Uveta.Extensions.Jobs.Endpoints.Extensions
{
    public static class GenericTypeExtensions
    {
        public static bool IsSubclassOfGeneric(this Type type, Type generic)
        {
            Type toCheck = type;
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static Type[] GetBaseGenericArguments(this Type type, Type baseType)
        {
            Type toCheck = type;
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (baseType == cur)
                {
                    return toCheck.GetGenericArguments().ToArray();
                }
                toCheck = toCheck.BaseType;
            }
            throw new ArgumentException($"Type {type.FullName} does not inherit from {baseType.FullName}");
        }

        public static Type[] GetInterfaceGenericArguments(this Type type, Type genericInterfaceType)
        {
            var interfaceType = type.GetInterface(genericInterfaceType.FullName);
            if (interfaceType is null) throw new ArgumentException($"Type {type.FullName} does not inherit from {genericInterfaceType.FullName}");
            return interfaceType.GetGenericArguments();
        }
    }
}
