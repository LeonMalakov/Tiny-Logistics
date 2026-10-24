using System.Collections;
using Code.Infrastructure.LifetimeScopes;
using Code.Infrastructure.SaveLoad.Model;
using Code.Infrastructure.SaveLoad.Services;
using Code.Infrastructure.SaveLoad.Storage;
using Code.Infrastructure.Serialization;
using Code.Infrastructure.States.App;
using Code.Simulation.Features.Production.Services;
using Code.Simulation.Features.ProductionUi.Windows;
using Code.Simulation.Features.Stockpile.Model;
using Code.Simulation.Features.Stockpile.Services;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace Tests.Editor.Integration.Startup {
    [Category("Integration")]
    public sealed class SaveLoadStartupTests : IsolatedSaveStartupTest {
        private const long SavedGold = 42;

        [UnityTest]
        public IEnumerator Enter_SavedProgress_RestoresEntitiesAndUiThroughLoadingStates() {
            using (GameTestContext seed = new()) {
                GameEntity factory = seed.CreateFactory();
                seed.Saves.CreateNewProgress();
                seed.Stockpile.Add(ItemId.Gold, SavedGold + seed.Production.UpgradeCost);

                Assert.That(seed.Production.TryUpgrade(), Is.True);

                factory.ReplaceCraftProgress(0.6f);
                factory.ReplaceHealth(180f);
                seed.Trucks.Create(new Vector3(3f, 0f, 2f)).ReplaceVelocity(Vector3.zero);
                seed.Enemies.Create(new Vector3(-5f, 0f, -8f));
                seed.SaveProgress();
                new ProgressSaveFileStore().WriteAllText(SavePath, seed.SaveStorage.ReadProgress());
            }

            EditorSceneManager.OpenScene(TestAssetPaths.BootScene);
            yield return new EnterPlayMode();
            Time.timeScale = 0f;

            try {
                yield return null;
                yield return null;
                AppLifetimeScope app = Object.FindAnyObjectByType<AppLifetimeScope>();

                Assert.That(app, Is.Not.Null);
                Assert.That(app.Container.Resolve<IProgressSaveStorage>().SavePath, Is.EqualTo(SavePath));

                IStockpileService stockpile = app.Container.Resolve<IStockpileService>();
                IProductionService production = app.Container.Resolve<IProductionService>();
                GameContext game = Contexts.sharedInstance.game;
                GameEntity factory = game.GetGroup(GameMatcher.Factory).GetSingleEntity();

                Assert.That(stockpile.GetAmount(ItemId.Gold), Is.EqualTo(SavedGold));
                Assert.That(production.UpgradeLevel, Is.EqualTo(1));
                Assert.That(factory.Health, Is.EqualTo(180f));
                Assert.That(factory.CraftProgress, Is.InRange(0.6f, 0.8f));
                Assert.That(factory.CraftDuration, Is.EqualTo(production.CraftDuration));
                Assert.That(factory.hasView, Is.True);
                Assert.That(game.GetGroup(GameMatcher.Truck).count, Is.EqualTo(1));
                Assert.That(game.GetGroup(GameMatcher.Enemy).count, Is.EqualTo(1));
                Assert.That(Object.FindAnyObjectByType<ProductionUi>().ResourcesLabel.text, Does.Contain($"Gold: {SavedGold}"));
                Assert.That(app.Container.Resolve<ISaveLoadService>().HasSavedProgress, Is.True);

                stockpile.Add(ItemId.Gold, 3);
                app.Container.Resolve<GameLoopState>().SaveProgress();
                string json = app.Container.Resolve<IProgressSaveStorage>().ReadProgress();
                ProgressData saved = JsonSerialization.Deserialize<ProgressData>(json);

                Assert.That(saved.Stockpile[ItemId.Gold], Is.EqualTo(SavedGold + 3),
                    "Immediate lifecycle saves must collect current service data through the same systems.");

                Assert.That(saved.ProductionUpgradeLevel, Is.EqualTo(1));
                Assert.That(saved.Entities.Count, Is.EqualTo(3));
            }
            finally {
                Time.timeScale = 1f;
            }

            yield return new ExitPlayMode();
        }
    }
}
