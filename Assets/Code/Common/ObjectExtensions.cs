using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Furkan.Common
{
    // ReSharper disable MemberCanBePrivate.Global
    public static class ObjectExtensions
    {
        // TODO: Why does NotNullWhen(bool) not work?

        /// <summary>
        /// Safe <c>is</c> operator with Unity lifetime check.
        /// </summary>
        public static bool Is<TSelf, TTarget>(this TSelf self, [MaybeNull, NotNullWhen(true)] out TTarget target)
            where TSelf : class
            where TTarget : Object
        {
            if (self.Missing())
            {
                target = null;
                return false;
            }

            target = self as TTarget;
            return (bool)target;
        }

        /// <summary>
        /// Safe <c>is not</c> operator with Unity lifetime check.
        /// </summary>
        public static bool IsNot<TSelf, TTarget>(this TSelf self, [MaybeNull, NotNullWhen(false)] out TTarget target)
            where TSelf : class
            where TTarget : Object
        {
            return !self.Is(out target);
        }

        /// <summary>
        /// Unity lifetime check (<c>self != null</c>).
        /// </summary>
        public static bool Exists<TSelf>(this TSelf self)
            where TSelf : class
        {
            return (bool)(self as Object);
        }

        /// <summary>
        /// Unity lifetime check (<c>self == null</c>).
        /// </summary>
        public static bool Missing<TSelf>(this TSelf self)
            where TSelf : class
        {
            return !self.Exists();
        }

        /// <summary>
        /// Safe <c>??</c> operator with Unity lifetime check.
        /// </summary>
        public static TSelf Or<TSelf>(this TSelf self, TSelf alternative)
            where TSelf : Object
        {
            return self ? self : alternative;
        }
    }
}
