using System;
using System.Collections.Generic;
using System.Text;
using Code.Common.Utility;
using Code.Infrastructure.Ecs;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Unity.Profiling;

namespace Code.Common.Ecs {
   public static class GameEntityPrinter {
        private static readonly StringBuilder Sb = new();
        private static readonly HashSet<Type> TypeBuffer = new();
        
        private static readonly ProfilerMarker TickProfilerMarker = new ProfilerMarker("Printer");
        
        public static string Print(this Entity entity) {
#if UNITY_EDITOR
            
            TickProfilerMarker.Begin();
            
            if (entity is GameEntity gameEntity) {
                if (gameEntity.hasRuntimeId) {
                    Sb.Append($"{{{gameEntity.RuntimeId}}} ");
                }
            }

            var components = entity.GetComponents();

            GetEntityName(components);
            
            string result = Sb.ToString();
            Sb.Clear();
            
            TickProfilerMarker.End();
            
            return result;
            
#else
            return string.Empty;
#endif
        }

        private static void GetEntityName(IComponent[] components) {
            if (components.Length == 0) {
                Sb.Append("NoComponents");
                return;
            }

            if (components.Length == 1) {
                Sb.Append(components[0].GetType().Name);
                return;
            }

            bool firstNameComponent = true;
            
            // Unique
            foreach (var x in components) {
                var type = x.GetType();

                if (TypeBuffer.Contains(type)) {
                    continue;
                }

                if (type.HasAttribute<UniqueAttribute>()) {

                    Sb.Append(firstNameComponent
                        ? $"{type.Name}"
                        : $" {type.Name}");

                    firstNameComponent = false;
                    TypeBuffer.Add(type);
                }
            }
            
            // EntityName
            foreach (var x in components) {
                var type = x.GetType();
                
                if (TypeBuffer.Contains(type)) {
                    continue;
                }

                if (type.HasAttribute<EntityNameAttribute>()) {

                    Sb.Append(firstNameComponent
                        ? $"{type.Name}"
                        : $" {type.Name}");

                    firstNameComponent = false;
                    TypeBuffer.Add(type);
                }
            }
            
            TypeBuffer.Clear();

            if (firstNameComponent) {
                Sb.Append("Unnamed");
            }
        }
    }
}
