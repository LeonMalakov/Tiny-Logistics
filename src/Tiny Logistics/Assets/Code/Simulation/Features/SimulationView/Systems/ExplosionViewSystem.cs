using System.Collections.Generic;
using Code.Simulation.Features.SimulationView.Factories;
using Entitas;

namespace Code.Simulation.Features.SimulationView.Systems {
    public sealed class ExplosionViewSystem : ReactiveSystem<GameEntity> {
        private readonly ISimulationViewFactory _views;

        public ExplosionViewSystem(GameContext game, ISimulationViewFactory views) : base(game) => _views = views;

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) =>
            context.CreateCollector(GameMatcher.Explosion.Added());
        
        protected override bool Filter(GameEntity entity) => entity.hasWorldPosition && entity.hasExplosionRadius;

        protected override void Execute(List<GameEntity> entities) {
            foreach (GameEntity entity in entities) {
                _views.CreateExplosionView(entity.WorldPosition, entity.ExplosionRadius);
            }
        }
    }
}
