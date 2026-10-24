using Code.Infrastructure.SaveLoad.Model;
using Code.Infrastructure.SaveLoad.Saving;
using Code.Infrastructure.SaveLoad.Saving.Systems;
using Code.Infrastructure.Serialization;
using Code.Simulation.Features.Stockpile.Model;
using Entitas;
using NUnit.Framework;
using Tests.Editor.Support;

namespace Tests.Editor.Integration.Persistence {
    [Category("Integration")]
    [Category("Persistence")]
    public sealed class SaveRequestTests {
        private GameTestContext _context;

        [SetUp]
        public void SetUp() => _context = new GameTestContext();

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void Execute_AutosaveInterval_CapturesCompletedStepAndWaitsBeforeSavingAgain() {
            _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            IExecuteSystem timer = _context.Systems.Create<PeriodicallySaveSystem>();
            SaveProgressFeature saving = _context.Systems.Create<SaveProgressFeature>();
            _context.Time.DeltaTime = 0f;
            timer.Execute();
            _context.Stockpile.Add(ItemId.Gold, 7);

            saving.Execute();

            Assert.That(_context.Progress.ProgressData.Stockpile[ItemId.Gold], Is.EqualTo(7));
            Assert.That(_context.SaveFiles.Writes, Is.EqualTo(1));

            _context.Time.DeltaTime = PeriodicallySaveSystem.AUTOSAVE_INTERVAL - 0.1f;
            timer.Execute();
            saving.Execute();

            Assert.That(_context.SaveFiles.Writes, Is.EqualTo(1));

            _context.Time.DeltaTime = 0.2f;
            timer.Execute();
            saving.Execute();

            Assert.That(_context.SaveFiles.Writes, Is.EqualTo(2));
        }

        [Test]
        public void Execute_MultipleSaveRequests_WritesOnceAndConsumesRequests() {
            _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            _context.Game.CreateEntity().isSaveProgressRequest = true;
            _context.Game.CreateEntity().isSaveProgressRequest = true;

            _context.Systems.Create<SaveProgressFeature>().Execute();

            Assert.That(_context.SaveFiles.Writes, Is.EqualTo(1));
            Assert.That(_context.Game.GetGroup(GameMatcher.SaveProgressRequest).count, Is.Zero);
        }

        [Test]
        public void Execute_FactoryDead_ClearsSavedProgressAndPreventsFurtherWrites() {
            GameEntity factory = _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            _context.SaveProgress();
            factory.ReplaceHealth(0f);

            _context.Systems.Create<SaveProgressFeature>().Execute();
            _context.Saves.SaveProgress();

            Assert.That(_context.Saves.HasSavedProgress, Is.False);
            Assert.That(_context.SaveFiles.Writes, Is.EqualTo(1));
            Assert.That(_context.Progress.ProgressData, Is.Null);
            Assert.That(_context.Saves.IsSessionReady, Is.False);
        }

        [Test]
        public void Execute_NoSaveRequest_KeepsSnapshotAndSkipsWriting() {
            _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            _context.Stockpile.Add(ItemId.Gold, 10);
            _context.SaveProgress();
            _context.Stockpile.Add(ItemId.Gold, 5);

            _context.Systems.Create<SaveProgressFeature>().Execute();

            Assert.That(_context.SaveFiles.Writes, Is.EqualTo(1));
            Assert.That(_context.Progress.ProgressData.Stockpile[ItemId.Gold], Is.EqualTo(10));
        }

        [Test]
        public void SaveProgress_LiveServicesChanged_WritesPreparedSnapshot() {
            _context.CreateFactory();
            _context.Saves.CreateNewProgress();
            long savedGold = _context.Production.UpgradeCost * 2;
            _context.Stockpile.Add(ItemId.Gold, savedGold);
            _context.SaveProgress();
            _context.Stockpile.Add(ItemId.Gold, _context.Production.UpgradeCost + 3);

            Assert.That(_context.Production.TryUpgrade(), Is.True);

            _context.Saves.SaveProgress();

            ProgressData saved = JsonSerialization.Deserialize<ProgressData>(_context.SaveStorage.ReadProgress());

            Assert.That(saved.Stockpile[ItemId.Gold], Is.EqualTo(savedGold));
            Assert.That(saved.ProductionUpgradeLevel, Is.Zero);
        }

        [Test]
        public void Execute_SessionNotInitialized_DiscardsSaveRequestsWithoutWriting() {
            _context.Game.CreateEntity().isSaveProgressRequest = true;
            _context.Systems.Create<SaveProgressFeature>().Execute();

            Assert.That(_context.SaveFiles.Writes, Is.Zero);
            Assert.That(_context.Game.GetGroup(GameMatcher.SaveProgressRequest).count, Is.Zero);
        }
    }
}
