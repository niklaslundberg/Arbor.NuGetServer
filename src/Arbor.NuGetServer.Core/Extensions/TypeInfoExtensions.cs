using System;
using System.Reflection;

namespace Arbor.NuGetServer.Core.Extensions
{
    public static class TypeInfoExtensions
    {
        public static bool IsPublicConcreteClassImplementing<T>(this Type typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            if (!typeInfo.IsClass)
            {
                return false;
            }

            if (typeInfo.IsAbstract)
            {
                return false;
            }

            Type type = typeof(T);

            bool isImplementingType = type.IsAssignableFrom(typeInfo);

            return isImplementingType;
        }
    }
}
