using System;
using System.Reflection;

namespace Arbor.NuGetServer.Core.Extensions
{
    public static class TypeInfoExtensions
    {
        public static bool IsPublicConcreteClassImplementing<T>(this TypeInfo typeInfo)
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

            TypeInfo type = typeof(T).GetTypeInfo();

            bool isImplementingType = type.IsAssignableFrom(typeInfo);

            return isImplementingType;
        }
    }
}