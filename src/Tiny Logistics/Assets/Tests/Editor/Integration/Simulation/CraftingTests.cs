using Code.Simulation.Features.Crafting.Systems;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEngine;

namespace Tests.Editor.Integration.Simulation {
    [Category("Integration")]
    [Category("Simulation")]
    public sealed class CraftingTests {
        private GameTestContext _context;

        [SetUp]
        public void SetUp() => _context = new GameTestContext();

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void Execute_MultipleCraftsCompleted_CreatesTrucksForFreeAndPreservesOverflow() {
            GameEntity factory = _context.CreateFactory();
            _context.Time.DeltaTime = _context.Production.CraftDuration * 2.5f;
            _context.Systems.Create<CraftSystem>().Execute();

            Assert.That(_context.Game.GetGroup(GameMatcher.Truck).count, Is.EqualTo(2));
            Assert.That(factory.CraftProgress, Is.EqualTo(0.5f).Within(0.0001f));
            Assert.That(_context.Stockpile.Items, Is.Empty);

            foreach (GameEntity truck in _context.Game.GetGroup(GameMatcher.Truck)) {
                Assert.That(truck.WorldPosition, Is.EqualTo(factory.WorldPosition + _context.World.TruckSpawnOffset));
                Assert.That(truck.Velocity, Is.EqualTo(Vector3.right * _context.Balance.TruckSpeed));
            }
        }

        [Test]
        public void Execute_FactoryHasZeroHealth_SkipsCraftingBeforeDeathIsProcessed() {
            GameEntity factory = _context.CreateFactory();
            factory.ReplaceHealth(0f);
            _context.Time.DeltaTime = _context.Production.CraftDuration * 2f;

            _context.Systems.Create<CraftSystem>().Execute();

            Assert.That(_context.Game.GetGroup(GameMatcher.Truck).count, Is.Zero);
        }
    }
}
