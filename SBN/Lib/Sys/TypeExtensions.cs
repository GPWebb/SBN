﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SBN.Lib.Sys
{
    public static class TypeExtensions
    {
        public static bool IsDerivedFromOpenGenericType(
            this Type type,
            Type openGenericType
        )
        {
            Contract.Requires(type != null);
            Contract.Requires(openGenericType != null);
            Contract.Requires(openGenericType.IsGenericTypeDefinition);
            return type.GetTypeHierarchy()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(t => openGenericType.Equals(t));
        }

        public static IEnumerable<Type> GetTypeHierarchy(this Type type)
        {
            Contract.Requires(type != null);
            Type currentType = type;
            while (currentType != null)
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }
        }

        public static bool IsNullable(this Type type)
        {
            Contract.Requires(type != null);
            return type.IsDerivedFromOpenGenericType(typeof(Nullable<>));
        }
    }
}
