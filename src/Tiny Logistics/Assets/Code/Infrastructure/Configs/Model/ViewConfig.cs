using Code.Common;
using Code.Simulation.Features.SimulationView.Views;
using UnityEngine;

namespace Code.Infrastructure.Configs.Model {
    [CreateAssetMenu(menuName = "Configs/ViewConfig")]
    public sealed class ViewConfig : ScriptableObject {
        [Header("Prefabs")]
        public SimulationEntityView FactoryPrefab;
        public SimulationEntityView TruckPrefab;
        public SimulationEntityView EnemyPrefab;
        public WorldView WorldPrefab;
        public ExplosionView ExplosionPrefab;
        
        [Header("Entity feedback")]
        public Color BurningColor = new(1f, 0.35f, 0.05f);
        public Color DeadColor = new(0.3f, 0.32f, 0.35f);
        [Min(0.01f)] public float BurningPulseSpeed = 4f;
        
        [Header("Explosions")]
        [Min(0.01f)] public float ExplosionDuration = 0.4f;

        public void Validate() {
            if (FactoryPrefab == null || TruckPrefab == null || EnemyPrefab == null || WorldPrefab == null ||
                ExplosionPrefab == null) {
                ThrowException.InvalidOperation(
                    "ViewConfig must reference factory, truck, enemy, world and explosion prefabs.");
            }

            FactoryPrefab.Validate();
            TruckPrefab.Validate();
            EnemyPrefab.Validate();
            WorldPrefab.Validate();
            ExplosionPrefab.Validate();
            
            ConfigValidation.Positive(BurningPulseSpeed, nameof(BurningPulseSpeed));
            ConfigValidation.Positive(ExplosionDuration, nameof(ExplosionDuration));
        }
    }
}
