using Code.Simulation.Features.SimulationView.Factories;
using Entitas;

namespace Code.Simulation.Features.SimulationView.Systems {
    public sealed class WorldViewSystem : IInitializeSystem, ITearDownSystem {
        private readonly ISimulationViewFactory _views;

        public WorldViewSystem(ISimulationViewFactory views) => _views = views;

        public void Initialize() => _views.CreateWorld();

        public void TearDown() => _views.ClearWorld();
    }
}
