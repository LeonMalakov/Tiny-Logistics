using UnityEngine;

namespace Code.Simulation.Features.Explosion.Factories {
    public interface IExplosionFactory {
        GameEntity Create(Vector3 position);
    }
}
