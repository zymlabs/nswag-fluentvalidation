using System;
using System.Linq;
using System.Reflection;

namespace ZymLabs.NSwag.FluentValidation
{
    /// <summary>
    /// Reflection extensions
    /// </summary>
    public static class ReflectionExtension
    {
        /// <summary>
        /// Is sub class of generic type
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsSubClassOfGeneric(this Type child, Type parent)
        {
            if (child == parent)
            {
                return false;
            }

            if (child.IsSubclassOf(parent))
            {
                return true;
            }

            var parameters = parent.GetGenericArguments();

            var isParameterLessGeneric = !(parameters.Length > 0 &&
                                           ((parameters[0].Attributes & TypeAttributes.BeforeFieldInit) ==
                                            TypeAttributes.BeforeFieldInit));

            while (child != typeof(object))
            {
                var cur = GetFullTypeDefinition(child);

                if (parent == cur || (isParameterLessGeneric && cur.GetInterfaces()
                        .Select(GetFullTypeDefinition)
                        .Contains(GetFullTypeDefinition(parent))))
                {
                    return true;
                }

                if (!isParameterLessGeneric)
                {
                    if (GetFullTypeDefinition(parent) == cur && !cur.IsInterface)
                    {
                        if (VerifyGenericArguments(GetFullTypeDefinition(parent), cur) &&
                            VerifyGenericArguments(parent, child))
                        {
                            return true;
                        }
                    }
                    else if (child.GetInterfaces()
                             .Where(i => GetFullTypeDefinition(parent) == GetFullTypeDefinition(i))
                             .Any(item => VerifyGenericArguments(parent, item)))
                    {
                        return true;
                    }
                }

                child = child.BaseType!;
            }

            return false;
        }

        private static Type GetFullTypeDefinition(Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        private static bool VerifyGenericArguments(Type parent, Type child)
        {
            Type[] childArguments = child.GetGenericArguments();
            Type[] parentArguments = parent.GetGenericArguments();

            if (childArguments.Length != parentArguments.Length)
            {
                return true;
            }

            return !childArguments.Where((t, i) =>
                    (t.Assembly != parentArguments[i].Assembly || t.Name != parentArguments[i].Name ||
                     t.Namespace != parentArguments[i].Namespace) && !t.IsSubclassOf(parentArguments[i]))
                .Any();
        }
    }
}