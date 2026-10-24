using Code.Infrastructure.Configs;
using Code.Infrastructure.Ecs;
using Code.Simulation.Features.SimulationView.Views;
using UnityEngine;

namespace Code.Simulation.Features.SimulationView.Factories {
    public sealed class SimulationViewFactory : ISimulationViewFactory {
        private readonly IConfigService _configs;
        private WorldView _world;

        public SimulationViewFactory(IConfigService configs) => _configs = configs;

        public void CreateWorld() {
            ClearWorld();
            _world = Object.Instantiate(_configs.View.WorldPrefab);
            _world.Validate();
        }

        public void CreateEntityView(GameEntity entity) {
            var config = _configs.View;
            
            SimulationEntityView prefab = entity.isFactory 
                ? config.FactoryPrefab 
                : entity.isTruck ? config.TruckPrefab : config.EnemyPrefab;
            
            SimulationEntityView view = Object.Instantiate(prefab, entity.WorldPosition, Quaternion.identity, _world.EntityRoot);

            view.Configure(config);
            view.SetEntity(entity);
            view.Refresh(0f);
        }

        public void CreateExplosionView(Vector3 position, float radius) {
            ExplosionView view = Object.Instantiate(_configs.View.ExplosionPrefab, position, Quaternion.identity, _world.EffectRoot);

            view.Configure(radius, _configs.View.ExplosionDuration);
        }

        public void ClearWorld() {
            if (_world == null) {
                return;
            }

            foreach (EntityView view in _world.GetComponentsInChildren<EntityView>(true)) {
                view.ReleaseEntity();
            }

            _world.gameObject.SetActive(false);
            PrimitiveGeometry.Destroy(_world.gameObject);
            _world = null;
        }
    }
}
