using System;
using Code.Infrastructure.Configs.Model;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor.Unit.Configs {
    [Category("Unit")]
    [Category("Configuration")]
    public sealed class ConfigValidationTests {
        [Test]
        public void Validate_UpgradeMultiplierDoesNotIncreaseSpeed_ThrowsArgumentOutOfRangeException() {
            BalanceConfig balance = ScriptableObject.CreateInstance<BalanceConfig>();

            try {
                balance.CraftSpeedMultiplierPerUpgrade = 1f;

                Assert.Throws<ArgumentOutOfRangeException>(() => balance.Validate());
            } finally {
                UnityEngine.Object.DestroyImmediate(balance);
            }
        }

        [Test]
        public void Validate_EnemySpawnAreaAboveTruckRoute_ThrowsArgumentException() {
            WorldConfig world = ScriptableObject.CreateInstance<WorldConfig>();

            try {
                world.EnemySpawnArea = new Rect(-3f, 1f, 5f, 1f);

                Assert.Throws<ArgumentException>(() => world.Validate());
            } finally {
                UnityEngine.Object.DestroyImmediate(world);
            }
        }
    }
}
