using Code.Common.ProcessDestructed.Systems;
using Code.Infrastructure.Ecs;
using Code.Simulation.Features.Explosion.Systems;
using Code.Simulation.Features.SimulationView.Factories;
using Code.Simulation.Features.SimulationView.Systems;
using Code.Simulation.Features.SimulationView.Views;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEngine;
using VContainer;

namespace Tests.Editor.Integration.Presentation {
    [Category("Integration")]
    [Category("Views")]
    public sealed class SimulationViewTests {
        private GameTestContext _context;

        [SetUp]
        public void SetUp() => _context = new GameTestContext(includeViews: true);

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void Execute_ExistingAndNewEntities_BindsExactlyOneViewEach() {
            GameEntity factory = _context.CreateFactory();
            _context.Container.Resolve<ISimulationViewFactory>().CreateWorld();
            CreateEntityViewSystem create = _context.Systems.Create<CreateEntityViewSystem>();
            create.Initialize();
            EntityView factoryView = factory.View;
            GameEntity truck = _context.Trucks.Create(Vector3.right);
            GameEntity enemy = _context.Enemies.Create(Vector3.back);

            create.Execute();
            create.Execute();

            Assert.That(Object.FindObjectsByType<SimulationEntityView>(FindObjectsSortMode.None).Length, Is.EqualTo(3));
            Assert.That(factory.View, Is.SameAs(factoryView));
            Assert.That(truck.View.Entity, Is.SameAs(truck));
            Assert.That(enemy.View.Entity, Is.SameAs(enemy));
        }

        [Test]
        public void Execute_PositionAndHealthChanged_UpdatesViewAndPreservesHealthBarLeftEdge() {
            _context.Container.Resolve<ISimulationViewFactory>().CreateWorld();
            GameEntity truck = _context.Trucks.Create(Vector3.zero);
            _context.Systems.Create<CreateEntityViewSystem>().Execute();
            SpriteRenderer fill = truck.View.transform.Find("Health Bar/Fill").GetComponent<SpriteRenderer>();
            float fullWidth = fill.transform.localScale.x;
            float leftEdge = fill.transform.localPosition.x + fill.sprite.bounds.min.x * fullWidth;
            truck.ReplaceWorldPosition(new Vector3(4f, 0f, 2f));
            truck.ReplaceHealth(truck.MaxHealth * 0.5f);

            _context.Systems.Create<SyncEntityViewSystem>().Execute();

            Assert.That(truck.View.transform.position, Is.EqualTo(truck.WorldPosition));
            Assert.That(fill.transform.localScale.x, Is.EqualTo(fullWidth * 0.5f).Within(0.0001f));
            Assert.That(fill.transform.localPosition.x + fill.sprite.bounds.min.x * fill.transform.localScale.x,
                Is.EqualTo(leftEdge).Within(0.0001f));
        }

        [Test]
        public void CreateEntityView_AuthoredPrefab_PreservesAppearanceAndHealthBarSize() {
            SimulationEntityView original = _context.View.TruckPrefab;
            SimulationEntityView template = Object.Instantiate(original);
            try {
                SpriteRenderer authoredBody = template.GetComponentInChildren<SpriteRenderer>();
                authoredBody.transform.localPosition = new Vector3(0.2f, 0.3f, -0.1f);
                authoredBody.transform.localScale *= 1.5f;
                template.transform.Find("Health Bar/Fill").localScale = new Vector3(2f, 0.1f, 1f);
                _context.View.TruckPrefab = template;
                _context.Container.Resolve<ISimulationViewFactory>().CreateWorld();
                GameEntity truck = _context.Trucks.Create(Vector3.zero);
                _context.Systems.Create<CreateEntityViewSystem>().Execute();
                SpriteRenderer spawnedBody = truck.View.GetComponentInChildren<SpriteRenderer>();

                Assert.That(spawnedBody.transform.localPosition, Is.EqualTo(authoredBody.transform.localPosition));
                Assert.That(spawnedBody.transform.localScale, Is.EqualTo(authoredBody.transform.localScale));
                Assert.That(spawnedBody.sharedMaterial, Is.SameAs(authoredBody.sharedMaterial));

                truck.ReplaceHealth(truck.MaxHealth * 0.25f);
                _context.Systems.Create<SyncEntityViewSystem>().Execute();

                Assert.That(truck.View.transform.Find("Health Bar/Fill").localScale.x, Is.EqualTo(0.5f).Within(0.0001f));
            } finally {
                _context.View.TruckPrefab = original;
                Object.DestroyImmediate(template.gameObject);
            }
        }

        [Test]
        public void Cleanup_DestructedEntity_ReleasesViewAndAllowsEntityReuse() {
            _context.Container.Resolve<ISimulationViewFactory>().CreateWorld();
            GameEntity truck = _context.Trucks.Create(Vector3.zero);
            _context.Systems.Create<CreateEntityViewSystem>().Execute();
            EntityView view = truck.View;
            truck.isDestructed = true;
            _context.Systems.Create<CleanupEntityViewSystem>().Cleanup();

            Assert.That(view == null, Is.True);
            Assert.That(truck.hasView, Is.False);

            _context.Systems.Create<CleanupDestructedSystem>().Cleanup();

            Assert.That(_context.Game.retainedEntitiesCount, Is.Zero);
            Assert.That(_context.Game.CreateEntity(), Is.SameAs(truck));
        }

        [Test]
        public void ReleaseEntity_RepeatedCallsAndDestroyedEntity_SafelyReleasesEntity() {
            _context.Container.Resolve<ISimulationViewFactory>().CreateWorld();
            GameEntity truck = _context.Trucks.Create(Vector3.zero);
            CreateEntityViewSystem create = _context.Systems.Create<CreateEntityViewSystem>();
            create.Execute();
            EntityView first = truck.View;
            first.ReleaseEntity();
            first.ReleaseEntity();
            Object.DestroyImmediate(first.gameObject);

            Assert.That(truck.hasView, Is.False);
            Assert.That(truck.isEnabled, Is.True);

            create.Execute();

            Assert.That(truck.View.Entity, Is.SameAs(truck));

            // Also release safely when ECS destruction happens before the view.
            EntityView view = truck.View;
            truck.Destroy();

            Assert.That(_context.Game.retainedEntitiesCount, Is.EqualTo(1));

            view.ReleaseEntity();
            Object.DestroyImmediate(view.gameObject);

            Assert.That(_context.Game.retainedEntitiesCount, Is.Zero);
        }

        [Test]
        public void Execute_ConsumedExplosion_CreatesOneEffectThatSurvivesEntityCleanup() {
            _context.Container.Resolve<ISimulationViewFactory>().CreateWorld();
            ExplosionViewSystem visuals = _context.Systems.Create<ExplosionViewSystem>();
            GameEntity explosion = _context.Explosions.Create(Vector3.zero);
            _context.Systems.Create<ExplosionSystem>().Execute();

            Assert.That(explosion.isDestructed, Is.True);

            visuals.Execute();
            visuals.Execute();
            _context.Systems.Create<CleanupDestructedSystem>().Cleanup();

            Assert.That(Object.FindObjectsByType<ExplosionView>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            Assert.That(explosion.isEnabled, Is.False);

            visuals.Deactivate();
            visuals.Clear();
        }
    }
}
