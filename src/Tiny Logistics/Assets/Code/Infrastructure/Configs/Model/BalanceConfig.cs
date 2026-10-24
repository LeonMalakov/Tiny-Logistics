using Code.Common;
using UnityEngine;

namespace Code.Infrastructure.Configs.Model {
    [CreateAssetMenu(menuName = "Configs/BalanceConfig")]
    public sealed class BalanceConfig : ScriptableObject {
        [Header("Factory")]
        [Min(0.01f)] public float InitialCraftDuration = 3f;
        [Min(0.01f)] public float MinCraftDuration = 0.5f;
        [Min(1)] public long ProductionUpgradeCost = 5;
        [Min(1.01f)] public float CraftSpeedMultiplierPerUpgrade = 1.25f;
        [Min(0.01f)] public float FactoryHealth = 250f;
        
        [Header("Trucks")]
        [Min(0.01f)] public float TruckSpeed = 1f;
        [Min(0.01f)] public float TruckHealth = 100f;
        [Min(1)] public long GoldPerTruck = 1;
        
        [Header("Enemies")]
        [Min(0.01f)] public float EnemyHealth = 40f;
        [Min(0.01f)] public float EnemySpeed = 2.5f;
        [Min(0.01f)] public float EnemySpawnInterval = 4f;
        [Min(0.01f)] public float EnemyAttackRange = 0.75f;
        
        [Header("Explosions and health")]
        [Min(0.01f)] public float ExplosionRadius = 3f;
        [Min(0)] public float ExplosionDamage = 20f;
        [Min(0.01f)] public float BurnDuration = 3f;
        [Min(0.01f)] public float BurnDamagePerSecond = 25f;
        [Min(0)] public float DeathDestructDelay = 1f;

        public void Validate() {
            ConfigValidation.Positive(InitialCraftDuration, nameof(InitialCraftDuration));
            ConfigValidation.Positive(MinCraftDuration, nameof(MinCraftDuration));
            ConfigValidation.Positive(FactoryHealth, nameof(FactoryHealth));
            ConfigValidation.Positive(TruckSpeed, nameof(TruckSpeed));
            ConfigValidation.Positive(TruckHealth, nameof(TruckHealth));
            ConfigValidation.Positive(CraftSpeedMultiplierPerUpgrade, nameof(CraftSpeedMultiplierPerUpgrade));
            ConfigValidation.Positive(EnemyHealth, nameof(EnemyHealth));
            ConfigValidation.Positive(EnemySpeed, nameof(EnemySpeed));
            ConfigValidation.Positive(EnemySpawnInterval, nameof(EnemySpawnInterval));
            ConfigValidation.Positive(EnemyAttackRange, nameof(EnemyAttackRange));
            ConfigValidation.Positive(ExplosionRadius, nameof(ExplosionRadius));
            ConfigValidation.NonNegative(ExplosionDamage, nameof(ExplosionDamage));
            ConfigValidation.Positive(BurnDuration, nameof(BurnDuration));
            ConfigValidation.Positive(BurnDamagePerSecond, nameof(BurnDamagePerSecond));
            ConfigValidation.NonNegative(DeathDestructDelay, nameof(DeathDestructDelay));
            
            if (MinCraftDuration > InitialCraftDuration) {
                ThrowException.ArgumentOutOfRange(nameof(MinCraftDuration));
            }

            if (ProductionUpgradeCost <= 0) {
                ThrowException.ArgumentOutOfRange(nameof(ProductionUpgradeCost));
            }
            
            if (CraftSpeedMultiplierPerUpgrade <= 1f) {
                ThrowException.ArgumentOutOfRange(nameof(CraftSpeedMultiplierPerUpgrade));
            }
            
            if (GoldPerTruck <= 0) {
                ThrowException.ArgumentOutOfRange(nameof(GoldPerTruck));
            }
        }
    }
}
