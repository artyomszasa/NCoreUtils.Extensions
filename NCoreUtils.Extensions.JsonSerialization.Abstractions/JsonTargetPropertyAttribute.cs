using System;

namespace NCoreUtils.JsonSerialization
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JsonTargetPropertyAttribute : Attribute
    {
        public string PropertyName { get; }

        public JsonTargetPropertyAttribute(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name must be non-empty string.", nameof(propertyName));
            }
            PropertyName = propertyName;
        }
    }
}