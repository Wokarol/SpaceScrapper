using System;
using UnityEngine;

namespace Wokarol.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SubclassSelectorAttribute : PropertyAttribute
    {
        Type m_type;

        public SubclassSelectorAttribute(System.Type type)
        {
            m_type = type;
        }

        public Type GetFieldType()
        {
            return m_type;
        }
    }
}
