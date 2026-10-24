using UnityEngine;

namespace Code.Simulation.Features.Enemies.Factories {
    public interface IEnemyFactory {
        GameEntity Create(UnityEngine.Vector3 position);
    }
}
