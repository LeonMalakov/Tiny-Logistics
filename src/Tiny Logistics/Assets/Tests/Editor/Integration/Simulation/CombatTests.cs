using Code.Common.ProcessDestructed.Systems;
using Code.Simulation.Features.Burning.Systems;
using Code.Simulation.Features.Explosion.Model;
using Code.Simulation.Features.Explosion.Systems;
using Code.Simulation.Features.Health.Systems;
using Entitas;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEngine;

namespace Tests.Editor.Integration.Simulation {
    [Category("Integration")]
    [Category("Simulation")]
    public sealed class CombatTests {
        private GameTestContext _context;

        [SetUp]
        public void SetUp() => _context = new GameTestContext();

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void Execute_PlayerExplosion_DamagesAndIgnitesEntitiesInRadius() {
            GameEntity factory = _context.CreateFactory();
            GameEntity truck = _context.Trucks.Create(factory.WorldPosition);
            GameEntity enemy = _context.Enemies.Create(factory.WorldPosition);
            GameEntity outside = _context.Enemies.Create(factory.WorldPosition + Vector3.right * (_context.Balance.ExplosionRadius + 1f));
            _context.Broker.Publish(new ExplodeAtRequest(factory.WorldPosition));
            ((IExecuteSystem)_context.Systems.Create<CreateExplosionSystem>()).Execute();
            _context.Systems.Create<ExplosionSystem>().Execute();
            foreach (GameEntity entity in new[] { factory, truck, enemy }) {
                Assert.That(entity.Health, Is.EqualTo(entity.MaxHealth - _context.Balance.ExplosionDamage));
                Assert.That(entity.isBurning, Is.True);
            }
            Assert.That(outside.isBurning, Is.False);
        }

        [Test]
        public void Execute_LethalExplosion_ClampsHealthAndDisablesFactory() {
            GameEntity factory = _context.CreateFactory();
            factory.ReplaceHealth(1f);
            _context.Explosions.Create(factory.WorldPosition);
            _context.Systems.Create<ExplosionSystem>().Execute();
            _context.Systems.Create<HealthSystem>().Execute();

            Assert.That(factory.Health, Is.Zero);
            Assert.That(factory.isDead, Is.True);
            Assert.That(_context.Production.IsFactoryAlive, Is.False);
        }

        [Test]
        public void Execute_BurningEndsMidFrame_AppliesRemainingDamageAndStopsBurning() {
            GameEntity enemy = _context.Enemies.Create(Vector3.zero);
            enemy.isBurning = true;
            enemy.AddBurningTimeLeft(0.1f);
            enemy.AddBurnDamagePerSecond(10f);
            _context.Time.DeltaTime = 2f;
            _context.Systems.Create<BurningSystem>().Execute();

            Assert.That(enemy.Health, Is.EqualTo(_context.Balance.EnemyHealth - 1f).Within(0.0001f));
            Assert.That(enemy.isBurning, Is.False);
            Assert.That(enemy.hasBurningTimeLeft, Is.False);
        }

        [Test]
        public void Execute_RepeatedDeathProcessing_PreservesDestructionDeadline() {
            GameEntity truck = _context.Trucks.Create(Vector3.zero);
            truck.ReplaceHealth(0f);
            _context.Systems.Create<HealthSystem>().Execute();
            _context.Systems.Create<SelfDestructTimerSystem>().Execute();
            _context.Systems.Create<HealthSystem>().Execute();

            Assert.That(truck.SelfDestructTimer, Is.EqualTo(_context.Balance.DeathDestructDelay - _context.Time.DeltaTime));

            _context.Time.DeltaTime = _context.Balance.DeathDestructDelay;
            _context.Systems.Create<SelfDestructTimerSystem>().Execute();
            _context.Systems.Create<CleanupDestructedSystem>().Cleanup();

            Assert.That(truck.isEnabled, Is.False);
        }
    }
}
