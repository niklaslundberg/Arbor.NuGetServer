using System;

namespace Arbor.NuGetServer.Api.Areas.CommonExtensions
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
