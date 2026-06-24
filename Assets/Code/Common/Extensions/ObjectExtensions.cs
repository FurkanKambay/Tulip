using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;

namespace Furkan.Common.Extensions
{
    [PublicAPI]
    public static class ObjectExtensions
    {
        // TODO: Figure out why `NotNullWhen(bool)` doesn't work

        /// <summary>
        /// Safe <c>is</c> operator with Unity lifetime check.
        /// </summary>
        [ContractAnnotation("self:null => false, target:null")]
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
        [ContractAnnotation("self:null => true, target:null")]
        public static bool IsNot<TSelf, TTarget>(this TSelf self, [MaybeNull, NotNullWhen(false)] out TTarget target)
            where TSelf : class
            where TTarget : Object
        {
            return !self.Is(out target);
        }

        /// <summary>
        /// Unity lifetime check (<c>self != null</c>).
        /// </summary>
        [ContractAnnotation("null => false")]
        public static bool Exists<TSelf>(this TSelf self)
            where TSelf : class
        {
            return (bool)(self as Object);
        }

        /// <summary>
        /// Unity lifetime check (<c>self == null</c>).
        /// </summary>
        [ContractAnnotation("null => true")]
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

        /// <summary>
        /// Safe <c>?? throw</c> with Unity lifetime check.
        /// </summary>
        [ContractAnnotation("self:null => halt")]
        public static TSelf OrThrow<TSelf, TException>(this TSelf self, TException exception)
            where TSelf : Object
            where TException : System.Exception
        {
            return self ? self : throw exception;
        }
    }
}
