using Code.Common.ProcessDestructed.Systems;
using Code.Simulation;
using Code.Simulation.Features.Stockpile.Model;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEngine;

namespace Tests.Editor.Integration.Simulation {
    [Category("Integration")]
    [Category("Simulation")]
    public sealed class SimulationPipelineTests {
        private GameTestContext _context;

        [SetUp]
        public void SetUp() => _context = new GameTestContext();

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void Execute_FactoryAliveThenDestroyed_CraftsAndDeliversUntilFactoryDies() {
            GameEntity factory = _context.CreateFactory();
            SimulationFeature feature = _context.Systems.Create<SimulationFeature>();
            SelfDestructTimerSystem destructionTimer = _context.Systems.Create<SelfDestructTimerSystem>();
            CleanupDestructedSystem cleanup = _context.Systems.Create<CleanupDestructedSystem>();
            feature.Initialize();
            _context.Time.DeltaTime = _context.Production.CraftDuration;
            feature.Execute();
            destructionTimer.Execute();
            cleanup.Cleanup();

            Assert.That(_context.Game.GetGroup(GameMatcher.Truck).count, Is.EqualTo(1));

            GameEntity truck = _context.Game.GetGroup(GameMatcher.Truck).GetSingleEntity();
            truck.ReplaceWorldPosition(new Vector3(_context.World.MapBounds.xMax + 1f, 0f, 0f));
            _context.Time.DeltaTime = 0.01f;
            feature.Execute();
            destructionTimer.Execute();
            cleanup.Cleanup();

            Assert.That(_context.Stockpile.GetAmount(ItemId.Gold), Is.EqualTo(_context.Balance.GoldPerTruck));

            factory.ReplaceHealth(0f);
            _context.Time.DeltaTime = 10f;
            feature.Execute();
            destructionTimer.Execute();
            cleanup.Cleanup();

            Assert.That(_context.Game.GetGroup(GameMatcher.Factory).count, Is.Zero);
            Assert.That(_context.Game.GetGroup(GameMatcher.Truck).count, Is.Zero);

            feature.TearDown();
        }
    }
}
