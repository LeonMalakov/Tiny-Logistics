using Code.Common.ProcessDestructed.Systems;
using Code.Simulation.Features.Enemies.Systems;
using Code.Simulation.Features.Explosion.Systems;
using Code.Simulation.Features.Movement.Systems;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEngine;

namespace Tests.Editor.Integration.Simulation {
    [Category("Integration")]
    [Category("Simulation")]
    public sealed class EnemyTests {
        private GameTestContext _context;

        [SetUp]
        public void SetUp() => _context = new GameTestContext();

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void Execute_NearestTruckDies_RetargetsNextLivingTruck() {
            GameEntity enemy = _context.Enemies.Create(Vector3.zero);
            GameEntity near = _context.Trucks.Create(new Vector3(1f, 0f, 0f));
            _context.Trucks.Create(new Vector3(0f, 0f, 5f));
            EnemyChaseSystem chase = _context.Systems.Create<EnemyChaseSystem>();
            chase.Execute();

            Assert.That(enemy.Velocity.x, Is.GreaterThan(0f));
            Assert.That(enemy.Velocity.z, Is.Zero);

            near.isDead = true;
            chase.Execute();

            Assert.That(enemy.Velocity.x, Is.Zero);
            Assert.That(enemy.Velocity.z, Is.GreaterThan(0f));
        }

        [Test]
        public void Execute_NoLivingTrucks_StopsEnemy() {
            GameEntity enemy = _context.Enemies.Create(Vector3.zero);
            enemy.ReplaceVelocity(Vector3.right);

            _context.Systems.Create<EnemyChaseSystem>().Execute();

            Assert.That(enemy.Velocity, Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void Execute_NearbyStationaryTruck_DoesNotOvershootTarget() {
            GameEntity enemy = _context.Enemies.Create(Vector3.zero);
            GameEntity truck = _context.Trucks.Create(new Vector3(0.1f, 0f, 0f));
            truck.ReplaceVelocity(Vector3.zero);
            _context.Time.DeltaTime = 1f;

            _context.Systems.Create<EnemyChaseSystem>().Execute();
            _context.Systems.Create<MovementSystem>().Execute();

            Assert.That(enemy.WorldPosition, Is.EqualTo(truck.WorldPosition));
        }

        [Test]
        public void Execute_LivingTrucksExist_SpawnsEnemiesBelowRouteOnlyWithTargets() {
            EnemySpawnSystem spawn = _context.Systems.Create<EnemySpawnSystem>();
            _context.Time.DeltaTime = _context.Balance.EnemySpawnInterval * 2f;
            spawn.Execute();

            Assert.That(_context.Game.GetGroup(GameMatcher.Enemy).count, Is.Zero);

            _context.Trucks.Create(Vector3.zero);
            spawn.Execute();

            Assert.That(_context.Game.GetGroup(GameMatcher.Enemy).count, Is.EqualTo(2));

            foreach (GameEntity enemy in _context.Game.GetGroup(GameMatcher.Enemy)) {
                Assert.That(enemy.WorldPosition.z, Is.LessThan(_context.World.FactoryPosition.z));
                Assert.That(_context.World.EnemySpawnArea.Contains(new Vector2(enemy.WorldPosition.x, enemy.WorldPosition.z)), Is.True);
            }
        }

        [Test]
        public void Execute_EnemyTouchesTruck_ExplodesOnceAndDestroysAttacker() {
            GameEntity truck = _context.Trucks.Create(Vector3.zero);
            GameEntity enemy = _context.Enemies.Create(new Vector3(0.5f, 0f, 0f));
            EnemyContactSystem contact = _context.Systems.Create<EnemyContactSystem>();
            contact.Execute();
            contact.Execute();

            Assert.That(enemy.isDestructed, Is.True);
            Assert.That(_context.Game.GetGroup(GameMatcher.Explosion).count, Is.EqualTo(1));

            _context.Systems.Create<ExplosionSystem>().Execute();

            Assert.That(truck.Health, Is.EqualTo(_context.Balance.TruckHealth - _context.Balance.ExplosionDamage));
            Assert.That(truck.isBurning, Is.True);

            _context.Systems.Create<CleanupDestructedSystem>().Cleanup();

            Assert.That(enemy.isEnabled, Is.False);
        }

        [Test]
        public void Execute_EnemyHasZeroHealth_DoesNotCreateExplosion() {
            GameEntity enemy = _context.Enemies.Create(Vector3.zero);
            enemy.ReplaceHealth(0f);
            _context.Trucks.Create(Vector3.zero);
            _context.Systems.Create<EnemyContactSystem>().Execute();

            Assert.That(_context.Game.GetGroup(GameMatcher.Explosion).count, Is.Zero);
        }
    }
}
