using UnityEngine;

namespace Code.Simulation.Features.SimulationView.Factories {
    public interface ISimulationViewFactory {
        void CreateWorld();

        void CreateEntityView(GameEntity entity);

        void CreateExplosionView(Vector3 position, float radius);

        void ClearWorld();
    }
}
