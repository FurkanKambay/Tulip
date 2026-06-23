using System;
using System.Diagnostics;
using UnityEngine;

namespace Furkan.Common
{
    [Conditional("UNITY_EDITOR")]
    public class RequiredAttribute : PropertyAttribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class ShowInInspectorAttribute : PropertyAttribute
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class OverlayRichLabelAttribute : PropertyAttribute
    {
        public OverlayRichLabelAttribute(string label)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class PostFieldRichLabelAttribute : PropertyAttribute
    {
        public PostFieldRichLabelAttribute(string label)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class DisableIfAttribute : PropertyAttribute
    {
        public DisableIfAttribute(string callback)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class EnableIfAttribute : PropertyAttribute
    {
        public EnableIfAttribute(string callback)
        {
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class ShowIfAttribute : PropertyAttribute
    {
        public ShowIfAttribute(string callback) { }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class LayoutGroupAttribute : PropertyAttribute
    {
        public LayoutGroupAttribute(string label, ELayout layout)
        {
        }
    }

    [Flags]
    public enum ELayout { FoldoutBox, TitleOut, Background, Foldout, Horizontal }

    public interface SaintsInterface<T1, T2>
    {
        public T2 I { get; set; }
    }
}
