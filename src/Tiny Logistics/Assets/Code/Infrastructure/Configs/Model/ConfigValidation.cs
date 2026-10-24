using Code.Common;
using UnityEngine;

namespace Code.Infrastructure.Configs.Model {
    internal static class ConfigValidation {
        public static void Positive(float value, string name) {
            Finite(value, name);
            if (value <= 0f)
                ThrowException.ArgumentOutOfRange(name);
        }
        
        public static void NonNegative(float value, string name) {
            Finite(value, name);
            if (value < 0f)
                ThrowException.ArgumentOutOfRange(name);
        }
        
        public static void Finite(float value, string name) {
            if (float.IsNaN(value) || float.IsInfinity(value))
                ThrowException.ArgumentOutOfRange(name);
        }
        
        public static void Rectangle(Rect value, string name) {
            Positive(value.width, name);
            Positive(value.height, name);
            Finite(value.xMin, name);
            Finite(value.xMax, name);
            Finite(value.yMin, name);
            Finite(value.yMax, name);
        }
        
        public static bool Contains(Rect bounds, Vector3 position) =>
            position.x >= bounds.xMin && position.x <= bounds.xMax &&
            position.z >= bounds.yMin && position.z <= bounds.yMax;
    }
}
