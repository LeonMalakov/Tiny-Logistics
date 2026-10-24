using System.Collections.Generic;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation;
using Code.Simulation.Features.Health.Model;
using Code.Simulation.Features.Health.Systems;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEngine;

namespace Tests.Editor.Integration.Presentation {
    [Category("Integration")]
    [Category("UI")]
    [Category("Views")]
    public sealed class UiEventOrderingTests {
        [Test]
        public void Execute_NonFactoryDeaths_DeliversEventsBeforeCleanupWithoutRefreshingUi() {
            using GameTestContext context = new(includeViews: true);
            context.CreateFactory();
            context.Balance.DeathDestructDelay = 0f;
            int uiChanges = 0;
            context.Ui.onDataChanged += () => uiChanges++;
            DeathListener listener = new();
            context.Dispatcher.AddListener<HealthUiEvents.EntityDied>(listener);
            GameEntity truck = context.Trucks.Create(Vector3.zero);
            GameEntity enemy = context.Enemies.Create(new Vector3(5f, 0f, -8f));
            truck.ReplaceHealth(0f);
            enemy.ReplaceHealth(0f);
            EveryFrameFeature frame = context.Systems.Create<EveryFrameFeature>();
            frame.Initialize();

            try {
                frame.Execute();

                Assert.That(listener.Entities, Is.EquivalentTo(new[] { truck, enemy }));
                Assert.That(listener.AllEntitiesWereEnabled, Is.True);
                Assert.That(truck.isEnabled && enemy.isEnabled, Is.True);
                Assert.That(uiChanges, Is.Zero);

                context.Systems.Create<HealthSystem>().Execute();
                context.Dispatcher.Flush();

                Assert.That(listener.Entities.Count, Is.EqualTo(2), "Each death must be published once.");

                frame.Cleanup();

                Assert.That(truck.isEnabled, Is.False);
                Assert.That(enemy.isEnabled, Is.False);
            } finally {
                frame.TearDown();
            }
        }

        private sealed class DeathListener : IUiEventListener<HealthUiEvents.EntityDied> {
            public List<GameEntity> Entities { get; } = new();
            public bool AllEntitiesWereEnabled { get; private set; } = true;

            public void OnUiEvent(HealthUiEvents.EntityDied eventData) {
                AllEntitiesWereEnabled &= eventData.Entity.isEnabled;
                Entities.Add(eventData.Entity);
            }
        }
    }
}
