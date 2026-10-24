using System;
using System.Collections.Generic;
using Code.Common;
using Code.Common.Identity;
using Code.Infrastructure.SaveLoad.Model;
using Code.Simulation.Features.Stockpile.Model;

namespace Code.Infrastructure.SaveLoad.Utility {
    public static class SaveLoadUtility {
        public static List<EntitySnapshotData> CaptureEntities(GameContext game) {
            List<EntitySnapshotData> snapshots = new();

            foreach (GameEntity entity in game.GetEntities()) {
                if (entity.isDestructed || (!entity.isFactory && !entity.isTruck && !entity.isEnemy)) {
                    continue;
                }

                snapshots.Add(new EntitySnapshotData {
                    Kind = entity.isFactory ? SavedEntityKind.Factory : entity.isTruck ? SavedEntityKind.Truck : SavedEntityKind.Enemy,
                    Position = new SavedVector3(entity.WorldPosition),
                    Health = entity.Health,
                    MaxHealth = entity.MaxHealth,
                    Dead = entity.isDead,
                    Velocity = entity.hasVelocity ? new SavedVector3(entity.Velocity) : null,
                    EnemySpeed = entity.hasEnemySpeed ? entity.EnemySpeed : null,
                    CraftProgress = entity.hasCraftProgress ? entity.CraftProgress : null,
                    BurningTimeLeft = entity.isBurning ? entity.BurningTimeLeft : null,
                    BurnDamagePerSecond = entity.isBurning ? entity.BurnDamagePerSecond : null,
                    SelfDestructTimeLeft = entity.hasSelfDestructTimer ? entity.SelfDestructTimer : null
                });
            }

            return snapshots;
        }

        public static void RestoreEntities(List<EntitySnapshotData> snapshots, GameContext game,
            IIdentityService identity, float craftDuration) {
            foreach (EntitySnapshotData saved in snapshots) {
                GameEntity entity = game.CreateEntity();
                entity.isFactory = saved.Kind == SavedEntityKind.Factory;
                entity.isTruck = saved.Kind == SavedEntityKind.Truck;
                entity.isEnemy = saved.Kind == SavedEntityKind.Enemy;
                entity.AddRuntimeId(identity.Next());
                entity.AddWorldPosition(saved.Position.ToVector3());
                entity.AddHealth(saved.Health);
                entity.AddMaxHealth(saved.MaxHealth);
                entity.isDead = saved.Dead;

                if (saved.Velocity.HasValue)
                    entity.AddVelocity(saved.Velocity.Value.ToVector3());

                if (saved.EnemySpeed.HasValue)
                    entity.AddEnemySpeed(saved.EnemySpeed.Value);

                if (saved.CraftProgress.HasValue) {
                    entity.AddCraftProgress(saved.CraftProgress.Value);
                    entity.AddCraftDuration(craftDuration);
                }

                if (saved.BurningTimeLeft.HasValue) {
                    entity.isBurning = true;
                    entity.AddBurningTimeLeft(saved.BurningTimeLeft.Value);
                    entity.AddBurnDamagePerSecond(saved.BurnDamagePerSecond.Value);
                }

                if (saved.SelfDestructTimeLeft.HasValue) {
                    entity.AddSelfDestructTimer(saved.SelfDestructTimeLeft.Value);
                }
            }
        }

        // Validate the complete snapshot before changing the running session.
        public static void Validate(ProgressData data) {
            Require(data != null && data.Version == ProgressData.CurrentVersion, "Unsupported save version.");
            Require(data.ProductionUpgradeLevel >= 0 && data.Stockpile != null && data.Entities != null, "Invalid session data.");

            foreach (var item in data.Stockpile) {
                Require(item.Key != ItemId.None && Enum.IsDefined(typeof(ItemId), item.Key) && item.Value >= 0,
                    "Invalid stockpile.");
            }

            int factories = 0;

            foreach (EntitySnapshotData entity in data.Entities) {
                Require(entity != null && Enum.IsDefined(typeof(SavedEntityKind), entity.Kind), "Invalid entity kind.");
                ValidateVector(entity.Position);
                Require(Finite(entity.Health) && Finite(entity.MaxHealth) && entity.MaxHealth > 0f &&
                        entity.Health >= 0f && entity.Health <= entity.MaxHealth, "Invalid entity health.");
                Require(entity.Dead == (entity.Health <= 0f), "Inconsistent death state.");
                Require(!entity.Dead || entity.SelfDestructTimeLeft.HasValue, "Dead entity has no removal timer.");

                if (entity.Kind == SavedEntityKind.Factory) {
                    factories++;
                    Require(!entity.Dead && entity.CraftProgress.HasValue, "The saved factory must be alive.");
                } else {
                    Require(entity.Velocity.HasValue, "Moving entity has no velocity.");
                }

                if (entity.Kind == SavedEntityKind.Enemy) {
                    Require(entity.EnemySpeed.HasValue, "Enemy has no speed.");
                }

                if (entity.Velocity.HasValue) {
                    ValidateVector(entity.Velocity.Value);
                }

                ValidateNonNegative(entity.EnemySpeed);
                ValidateNonNegative(entity.CraftProgress);
                ValidateNonNegative(entity.SelfDestructTimeLeft);
                Require(!entity.CraftProgress.HasValue || entity.CraftProgress.Value < 1f, "Invalid craft progress.");
                Require(entity.BurningTimeLeft.HasValue == entity.BurnDamagePerSecond.HasValue, "Incomplete burning state.");

                if (entity.BurningTimeLeft.HasValue) {
                    Require(!entity.Dead && Finite(entity.BurningTimeLeft.Value) && entity.BurningTimeLeft.Value > 0f,
                        "Invalid burning duration.");
                    ValidateNonNegative(entity.BurnDamagePerSecond);
                }
            }

            Require(factories == 1, "A saved session must contain one living factory.");
        }

        private static void ValidateVector(SavedVector3 value) =>
            Require(Finite(value.X) && Finite(value.Y) && Finite(value.Z), "Invalid vector.");

        private static void ValidateNonNegative(float? value) =>
            Require(!value.HasValue || (Finite(value.Value) && value.Value >= 0f), "Invalid numeric value.");

        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        private static void Require(bool condition, string message) {
            if (!condition)
                ThrowException.InvalidData(message);
        }
    }
}
