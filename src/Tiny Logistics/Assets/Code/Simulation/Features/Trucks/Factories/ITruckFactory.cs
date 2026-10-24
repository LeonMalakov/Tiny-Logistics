using UnityEngine;

namespace Code.Simulation.Features.Trucks.Factories {
    public interface ITruckFactory {
        GameEntity Create(Vector3 position);
    }
}
