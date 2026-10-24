using System.Collections;
using Code.Infrastructure.Configs;
using Code.Infrastructure.LifetimeScopes;
using Code.Simulation.Features.Enemies.Factories;
using Code.Simulation.Features.Production.Services;
using Code.Simulation.Features.ProductionUi.Windows;
using Code.Simulation.Features.SimulationView.Views;
using Code.Simulation.Features.Stockpile.Model;
using Code.Simulation.Features.Stockpile.Services;
using Code.Simulation.Features.Trucks.Factories;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace Tests.Editor.Integration.Startup {
    [Category("Integration")]
    public sealed class GameRestartStartupTests : IsolatedSaveStartupTest {
        [UnityTest]
        public IEnumerator OnTick_FactoryDestroyed_ReloadsSceneAndResetsSessionAcrossRestarts() {
            EditorSceneManager.OpenScene(TestAssetPaths.BootScene);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            Time.timeScale = 1f;
            AppLifetimeScope app = Object.FindAnyObjectByType<AppLifetimeScope>();
            IConfigService configs = app.Container.Resolve<IConfigService>();
            IStockpileService stockpile = app.Container.Resolve<IStockpileService>();
            IProductionService production = app.Container.Resolve<IProductionService>();
            GameContext game = Contexts.sharedInstance.game;

            for (int restart = 0; restart < 2; restart++) {
                GameEntity factory = game.GetGroup(GameMatcher.Factory).GetSingleEntity();
                int oldFactoryId = factory.RuntimeId;
                int oldSceneScopeId = Object.FindAnyObjectByType<SimulationLifetimeScope>().GetInstanceID();
                var oldFactoryView = factory.View;
                stockpile.Add(ItemId.Gold, production.UpgradeCost * 2);

                Assert.That(production.TryUpgrade(), Is.True);
                Assert.That(production.CraftDuration, Is.LessThan(configs.Balance.InitialCraftDuration));

                GameEntity truck = app.Container.Resolve<ITruckFactory>().Create(Vector3.zero);
                GameEntity enemy = app.Container.Resolve<IEnemyFactory>().Create(new Vector3(5f, 0f, -8f));
                yield return null;
                var oldTruckView = truck.View;
                var oldEnemyView = enemy.View;
                factory.ReplaceHealth(0f);

                float deadline = Time.realtimeSinceStartup + 15f;
                while (!factory.isDead && Time.realtimeSinceStartup < deadline) {
                    yield return null;
                }

                Assert.That(factory.isDead, Is.True);
                Assert.That(factory.hasSelfDestructTimer, Is.True);

                GameEntity newFactory = null;
                while (Time.realtimeSinceStartup < deadline) {
                    var factories = game.GetGroup(GameMatcher.Factory);
                    if (factories.count == 1) {
                        GameEntity candidate = factories.GetSingleEntity();
                        if (candidate.RuntimeId != oldFactoryId && candidate.hasView) {
                            newFactory = candidate;
                            break;
                        }
                    }
                    yield return null;
                }
                Assert.That(newFactory, Is.Not.Null, "The state machine must initialize a new session after factory removal.");

                yield return null;

                Assert.That(Object.FindAnyObjectByType<AppLifetimeScope>(), Is.SameAs(app));
                Assert.That(Object.FindAnyObjectByType<SimulationLifetimeScope>().GetInstanceID(), Is.Not.EqualTo(oldSceneScopeId));
                Assert.That(newFactory.Health, Is.EqualTo(configs.Balance.FactoryHealth));
                Assert.That(newFactory.isDead, Is.False);
                Assert.That(newFactory.isBurning, Is.False);
                Assert.That(newFactory.CraftDuration, Is.EqualTo(configs.Balance.InitialCraftDuration));
                Assert.That(production.CraftDuration, Is.EqualTo(configs.Balance.InitialCraftDuration));
                Assert.That(stockpile.GetAmount(ItemId.Gold), Is.Zero);
                Assert.That(Object.FindAnyObjectByType<ProductionUi>().UpgradeButton.interactable, Is.False);
                Assert.That(oldFactoryView == null && oldTruckView == null && oldEnemyView == null, Is.True);
                Assert.That(game.GetGroup(GameMatcher.Truck).count, Is.Zero);
                Assert.That(game.GetGroup(GameMatcher.Enemy).count, Is.Zero);
                Assert.That(game.retainedEntitiesCount, Is.Zero);
                Assert.That(Object.FindObjectsByType<AppLifetimeScope>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
                Assert.That(Object.FindObjectsByType<SimulationLifetimeScope>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
                Assert.That(Object.FindObjectsByType<ProductionUi>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
                Assert.That(Object.FindObjectsByType<WorldView>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
                Assert.That(Object.FindObjectsByType<SimulationEntityView>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            }
            yield return new ExitPlayMode();
        }
    }
}
