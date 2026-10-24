using System;

namespace Code.Common.Utility {
    public static class ReflectionUtility {
        public static bool HasAttribute<T>(this Type type) where T : System.Attribute {
            return type.GetCustomAttributes(typeof(T), true).Length > 0;
        }
        
        public static bool TryGetAttribute<T>(this Type type, out T attribute) where T : System.Attribute {
            object[] attributes = type.GetCustomAttributes(typeof(T), true);
            
            if (attributes.Length == 0) {
                attribute = null;
                return false;
            }
            
            attribute = (T) attributes[0];
            return true;
        }
    }
}