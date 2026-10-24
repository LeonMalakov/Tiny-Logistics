using System.Text.RegularExpressions;
using Code.Infrastructure.SaveLoad.Model;
using Code.Infrastructure.Serialization;
using Code.Simulation.Features.Burning.Systems;
using Code.Simulation.Features.Stockpile.Model;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.Integration.Persistence {
    [Category("Integration")]
    [Category("Persistence")]
    public sealed class SaveLoadTests {
        private GameTestContext _context;

        [SetUp]
        public void SetUp() => _context = new GameTestContext();

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void SaveAndLoadProgress_UpgradedFactory_RestoresFactoryStockpileAndProduction() {
            GameEntity factory = _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            factory.ReplaceCraftProgress(0.65f);
            factory.ReplaceHealth(190f);
            _context.Stockpile.Add(ItemId.Gold, (long)int.MaxValue + 20);
            _context.Production.RestoreUpgradeLevel(2);
            long gold = _context.Stockpile.GetAmount(ItemId.Gold);
            float duration = _context.Production.CraftDuration;

            SaveAndReload();

            GameEntity restored = _context.Game.GetGroup(GameMatcher.Factory).GetSingleEntity();

            Assert.That(restored.CraftProgress, Is.EqualTo(0.65f));
            Assert.That(restored.CraftDuration, Is.EqualTo(duration));
            Assert.That(restored.Health, Is.EqualTo(190f));
            Assert.That(restored.hasView, Is.False);
            Assert.That(_context.Stockpile.GetAmount(ItemId.Gold), Is.EqualTo(gold));
            Assert.That(_context.Production.UpgradeLevel, Is.EqualTo(2));
        }

        [Test]
        public void SaveAndLoadProgress_BurningFactory_ResumesBurningFromSavedState() {
            GameEntity factory = _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            factory.ReplaceHealth(190f);
            factory.isBurning = true;
            factory.AddBurningTimeLeft(1.25f);
            factory.AddBurnDamagePerSecond(10f);

            SaveAndReload();

            GameEntity restored = _context.Game.GetGroup(GameMatcher.Factory).GetSingleEntity();

            Assert.That(restored.isBurning, Is.True);
            Assert.That(restored.BurningTimeLeft, Is.EqualTo(1.25f));
            Assert.That(restored.BurnDamagePerSecond, Is.EqualTo(10f));

            _context.Time.DeltaTime = 0.25f;
            _context.Systems.Create<BurningSystem>().Execute();

            Assert.That(restored.Health, Is.EqualTo(187.5f));
            Assert.That(restored.BurningTimeLeft, Is.EqualTo(1f));
        }

        [Test]
        public void SaveAndLoadProgress_MixedEntities_RestoresPersistentStateAndExcludesExplosions() {
            _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            GameEntity truck = _context.Trucks.Create(new Vector3(4f, 0f, 2f));
            truck.ReplaceHealth(45f);
            truck.ReplaceVelocity(new Vector3(2f, 0f, 0f));
            GameEntity dead = _context.Trucks.Create(new Vector3(6f, 0f, 1f));
            dead.ReplaceHealth(0f);
            dead.isDead = true;
            dead.AddSelfDestructTimer(0.4f);
            _context.Enemies.Create(new Vector3(-2f, 0f, -8f)).ReplaceEnemySpeed(3.5f);
            _context.Explosions.Create(Vector3.zero);

            SaveAndReload();

            GameEntity restoredTruck = _context.Game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Truck)
                .NoneOf(
                    GameMatcher.Dead)).GetSingleEntity();
            GameEntity restoredDead = _context.Game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Truck,
                    GameMatcher.Dead)).GetSingleEntity();
            GameEntity restoredEnemy = _context.Game.GetGroup(GameMatcher.Enemy).GetSingleEntity();

            Assert.That(_context.Game.count, Is.EqualTo(4), "Transient explosions must not be restored.");
            Assert.That(restoredTruck.WorldPosition, Is.EqualTo(new Vector3(4f, 0f, 2f)));
            Assert.That(restoredTruck.Velocity, Is.EqualTo(new Vector3(2f, 0f, 0f)));
            Assert.That(restoredTruck.Health, Is.EqualTo(45f));
            Assert.That(restoredDead.Health, Is.Zero);
            Assert.That(restoredDead.SelfDestructTimer, Is.EqualTo(0.4f));
            Assert.That(restoredEnemy.WorldPosition, Is.EqualTo(new Vector3(-2f, 0f, -8f)));
            Assert.That(restoredEnemy.EnemySpeed, Is.EqualTo(3.5f));
        }

        [TestCase("{}")]
        [TestCase("null")]
        [TestCase("{broken")]
        [TestCase("{\"Version\":99,\"ProductionUpgradeLevel\":0,\"Stockpile\":{},\"Entities\":[]}")]
        public void LoadProgress_UnreadableOrUnsupportedSave_DiscardsSnapshot(string json) {
            _context.SaveStorage.WriteProgress(json);

            LogAssert.Expect(LogType.Warning, new Regex("\\[Save\\] Cannot load session"));
            _context.Saves.LoadProgress();

            Assert.That(_context.RestoreProgress(), Is.False);
            Assert.That(_context.Progress.ProgressData, Is.Null);
            Assert.That(_context.Game.count, Is.Zero);
        }

        [Test]
        public void LoadProgress_InvalidSnapshot_RejectsSnapshotWithoutRestoringState() {
            _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            _context.Stockpile.Add(ItemId.Gold, 17);
            _context.SaveProgress();
            ProgressData data = _context.Progress.ProgressData;
            data.Stockpile[ItemId.Gold] = -1;
            _context.SaveStorage.WriteProgress(JsonSerialization.Serialize(data));

            LogAssert.Expect(LogType.Warning, new Regex("\\[Save\\] Cannot load session"));
            _context.Saves.LoadProgress();

            Assert.That(_context.RestoreProgress(), Is.False);
            Assert.That(_context.Stockpile.GetAmount(ItemId.Gold), Is.EqualTo(17));
            Assert.That(_context.Game.count, Is.EqualTo(1));
        }

        private void SaveAndReload() {
            _context.SaveProgress();
            _context.Game.DestroyAllEntities();
            _context.Stockpile.Clear();
            _context.Production.Reset();
            _context.Saves.LoadProgress();

            Assert.That(_context.Game.count, Is.Zero, "Reading JSON must not instantiate entities before loading systems run.");
            Assert.That(_context.RestoreProgress(), Is.True);
        }
    }
}
