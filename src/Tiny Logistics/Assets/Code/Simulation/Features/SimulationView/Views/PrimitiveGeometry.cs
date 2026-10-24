using UnityEngine;

namespace Code.Simulation.Features.SimulationView.Views {
    public static class PrimitiveGeometry {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        public static void Tint(Renderer renderer, Color color, MaterialPropertyBlock block) {
            block.SetColor(BaseColor, color);
            renderer.SetPropertyBlock(block);
        }

        public static void Destroy(Object target) {
            if (target == null)
                return;

            if (Application.isPlaying) {
                Object.Destroy(target);
            } else {
                Object.DestroyImmediate(target);
            }
        }
        
    }
}
