using UnityEngine;

namespace Code.Simulation.Features.Explosion.Model {
    public readonly struct ExplodeAtRequest {
        public readonly Vector3 Position;

        public ExplodeAtRequest(Vector3 position) {
            Position = position;
        }
    }
}
