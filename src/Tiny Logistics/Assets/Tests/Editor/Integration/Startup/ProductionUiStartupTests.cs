using System.Collections;
using System.Globalization;
using Code.Infrastructure.Configs;
using Code.Infrastructure.LifetimeScopes;
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
    public sealed class ProductionUiStartupTests : IsolatedSaveStartupTest {
        [UnityTest]
        public IEnumerator OnPointerClick_UpgradeButton_UpgradesProductionWithoutIgnitingEntitiesBehindUi() {
            EditorSceneManager.OpenScene(TestAssetPaths.BootScene);
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            Time.timeScale = 0f;

            AppLifetimeScope scope = Object.FindAnyObjectByType<AppLifetimeScope>();
            IStockpileService stockpile = scope.Container.Resolve<IStockpileService>();
            IProductionService production = scope.Container.Resolve<IProductionService>();
            ProductionUi view = Object.FindAnyObjectByType<ProductionUi>();
            float initialDuration = production.CraftDuration;

            Assert.That(view, Is.Not.Null);
            Assert.That(view.UpgradeButton.interactable, Is.False);

            stockpile.Add(ItemId.Gold, production.UpgradeCost * 2);
            yield return null;
            yield return null;

            Assert.That(view.UpgradeButton.interactable, Is.True);

            Canvas.ForceUpdateCanvases();
            RectTransform button = (RectTransform)view.UpgradeButton.transform;
            Vector2 pointer = RectTransformUtility.WorldToScreenPoint(null, button.TransformPoint(button.rect.center));
            Ray ray = Camera.main.ScreenPointToRay(pointer);
            Plane ground = new(Vector3.up, scope.Container.Resolve<IConfigService>().World.FactoryPosition);

            Assert.That(ground.Raycast(ray, out float distance), Is.True);

            GameEntity targetUnderUi = Contexts.sharedInstance.game.CreateEntity();
            targetUnderUi.AddWorldPosition(ray.GetPoint(distance));
            targetUnderUi.AddHealth(100f);

            using (TestMouse mouse = new()) {
                mouse.MoveTo(pointer);
                yield return null;
                yield return null;
                mouse.PressAt(pointer);
                yield return null;
                yield return null;
                mouse.MoveTo(pointer);
                yield return null;
                yield return null;
            }

            Assert.That(production.CraftDuration, Is.EqualTo(initialDuration / production.UpgradeSpeedMultiplier).Within(0.0001f));
            Assert.That(stockpile.GetAmount(ItemId.Gold), Is.EqualTo(production.UpgradeCost));
            Assert.That(view.ResourcesLabel.text, Does.Contain($"Gold: {production.UpgradeCost}"));
            Assert.That(view.ProductionLabel.text, Does.Contain(production.CraftDuration.ToString("0.##", CultureInfo.InvariantCulture)));
            Assert.That(targetUnderUi.Health, Is.EqualTo(100f));
            Assert.That(targetUnderUi.isBurning, Is.False);

            stockpile.TryConsume(ItemId.Gold, production.UpgradeCost);
            yield return null;
            yield return null;

            Assert.That(view.UpgradeButton.interactable, Is.False);

            yield return new ExitPlayMode();
        }
    }
}
