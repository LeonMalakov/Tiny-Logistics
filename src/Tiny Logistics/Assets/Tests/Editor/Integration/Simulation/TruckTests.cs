using Code.Common.ProcessDestructed.Systems;
using Code.Simulation.Features.Movement.Systems;
using Code.Simulation.Features.Stockpile.Model;
using Code.Simulation.Features.Trucks.Systems;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEngine;

namespace Tests.Editor.Integration.Simulation {
    [Category("Integration")]
    [Category("Simulation")]
    public sealed class TruckTests {
        private GameTestContext _context;

        [SetUp]
        public void SetUp() => _context = new GameTestContext();

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void Execute_LivingAndDeadTrucks_MovesOnlyLivingTruck() {
            GameEntity truck = _context.Trucks.Create(Vector3.zero);
            GameEntity dead = _context.Trucks.Create(Vector3.zero);
            dead.isDead = true;
            _context.Systems.Create<MovementSystem>().Execute();

            Assert.That(truck.WorldPosition, Is.EqualTo(Vector3.right * _context.Balance.TruckSpeed * _context.Time.DeltaTime));
            Assert.That(dead.WorldPosition, Is.EqualTo(Vector3.zero));
        }

        [TestCase(0f, false)]
        [TestCase(0.01f, true)]
        public void Execute_TruckAtOrBeyondBoundary_PaysOnceOnlyAfterLeavingMap(float offsetFromBoundary, bool outside) {
            GameEntity truck = _context.Trucks.Create(new Vector3(_context.World.MapBounds.xMax + offsetFromBoundary, 0f, 0f));
            TruckDeliverySystem delivery = _context.Systems.Create<TruckDeliverySystem>();
            delivery.Execute();
            delivery.Execute();

            Assert.That(_context.Stockpile.GetAmount(ItemId.Gold), Is.EqualTo(outside ? _context.Balance.GoldPerTruck : 0));
            Assert.That(truck.isDestructed, Is.EqualTo(outside));

            _context.Systems.Create<CleanupDestructedSystem>().Cleanup();

            Assert.That(truck.isEnabled, Is.EqualTo(!outside));
        }

        [Test]
        public void Execute_TruckHasZeroHealth_DoesNotPayGold() {
            GameEntity truck = _context.Trucks.Create(new Vector3(_context.World.MapBounds.xMax + 1f, 0f, 0f));
            truck.ReplaceHealth(0f);
            _context.Systems.Create<TruckDeliverySystem>().Execute();

            Assert.That(_context.Stockpile.Items, Is.Empty);
        }
    }
}
