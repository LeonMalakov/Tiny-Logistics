using System.Collections;
using Code.Infrastructure.Configs;
using Code.Infrastructure.LifetimeScopes;
using Code.Infrastructure.SaveLoad.Storage;
using Code.Simulation.Features.ProductionUi.Windows;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace Tests.Editor.Integration.Startup {
    [Category("Integration")]
    public sealed class DemoStartupTests : IsolatedSaveStartupTest {
        [UnityTest]
        public IEnumerator Enter_NoSavedProgress_LoadsConfigsBindsFactoryViewAndStartsSimulation() {
            EditorSceneManager.OpenScene(TestAssetPaths.BootScene);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            GameContext game = Contexts.sharedInstance.game;
            AppLifetimeScope scope = Object.FindAnyObjectByType<AppLifetimeScope>();

            Assert.That(scope, Is.Not.Null);
            Assert.That(scope.Container.Resolve<ProgressSavePaths>().PersistentDataPath, Is.EqualTo(SaveDirectory));

            IConfigService configs = scope.Container.Resolve<IConfigService>();

            Assert.That(configs.Balance, Is.SameAs(scope.AssetRepository.Configs.Balance));
            Assert.That(configs.World, Is.SameAs(scope.AssetRepository.Configs.World));
            Assert.That(game.GetGroup(GameMatcher.Factory).count, Is.EqualTo(1));

            GameEntity factory = game.GetGroup(GameMatcher.Factory).GetSingleEntity();

            Assert.That(factory.Health, Is.EqualTo(configs.Balance.FactoryHealth));
            Assert.That(factory.hasView, Is.True);
            Assert.That(factory.View.Entity, Is.SameAs(factory));

            ProductionUi view = Object.FindAnyObjectByType<ProductionUi>();

            Assert.That(view.ResourcesLabel.text, Does.Contain("Gold:"));
            Assert.That(view.ProductionLabel.text, Does.Contain("Truck"));

            Time.timeScale = 10f;
            float deadline = Time.time + configs.Balance.InitialCraftDuration + 2f;
            while (game.GetGroup(GameMatcher.Truck).count == 0 && Time.time < deadline) {
                yield return null;
            }

            Assert.That(game.GetGroup(GameMatcher.Truck).count, Is.GreaterThanOrEqualTo(1));

            deadline = Time.time + configs.Balance.EnemySpawnInterval + 2f;
            while (game.GetGroup(GameMatcher.Enemy).count == 0 && Time.time < deadline) {
                yield return null;
            }

            Assert.That(game.GetGroup(GameMatcher.Enemy).count, Is.GreaterThanOrEqualTo(1));

            Time.timeScale = 1f;
            yield return new ExitPlayMode();
        }
    }
}
