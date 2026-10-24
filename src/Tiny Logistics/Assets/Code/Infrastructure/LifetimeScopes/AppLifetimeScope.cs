using Code.Common.Identity;
using Code.Common.Time;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Broker;
using Code.Infrastructure.Configs;
using Code.Infrastructure.Ecs.Systems;
using Code.Infrastructure.Loading;
using Code.Infrastructure.SaveLoad.Services;
using Code.Infrastructure.SaveLoad.Storage;
using Code.Infrastructure.States.App;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.Enemies.Factories;
using Code.Simulation.Features.Explosion.Factories;
using Code.Simulation.Features.Production.Services;
using Code.Simulation.Features.ProductionUi.UiServices;
using Code.Simulation.Features.SimulationView.Factories;
using Code.Simulation.Features.Stockpile.Services;
using Code.Simulation.Features.Trucks.Factories;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Code.Infrastructure.LifetimeScopes {
    public class AppLifetimeScope : LifetimeScope {
        [Header("Dependencies")]
        public AssetRepository AssetRepository;

        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);
            DontDestroyOnLoad(this);

            BindEntryPoint(builder);
            BindCommonServices(builder);
            BindAppStateMachine(builder);
            BindAppStates(builder);
            BindAssetRepository(builder);
            BindSimulationFactories(builder);
            BindGameplayServices(builder);
            BindUiServices(builder);
            BindProgressServices(builder);
            BindEcsContexts(builder);
            BindSystemFactory(builder);
        }

        private static void BindEntryPoint(IContainerBuilder builder) {
            builder.RegisterEntryPoint<AppBootstrapper>();
        }

        private void BindCommonServices(IContainerBuilder builder) {
            builder.Register<ISceneLoadService, SceneLoadService>(Lifetime.Singleton);
            builder.Register<IIdentityService, IdentityService>(Lifetime.Singleton);
            builder.Register<ITimeService, TimeService>(Lifetime.Singleton);

            builder.Register<IRequestBroker, RequestBroker>(Lifetime.Singleton);
            builder.Register<IUiDispatcher, UiDispatcher.UiDispatcher>(Lifetime.Singleton);
        }

        private static void BindAppStateMachine(IContainerBuilder builder) {
            builder.Register<AppStateMachine>(Lifetime.Singleton);
            builder.Register<AppStateFactory>(Lifetime.Singleton);
        }

        private static void BindAppStates(IContainerBuilder builder) {
            builder.Register<BootState>(Lifetime.Singleton);
            builder.Register<GameLoadingState>(Lifetime.Singleton);
            builder.Register<GameInitializeState>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameLoopState>(Lifetime.Singleton).AsSelf();
        }

        private void BindAssetRepository(IContainerBuilder builder) {
            builder.RegisterInstance(AssetRepository).AsImplementedInterfaces();
        }

        private static void BindProgressServices(IContainerBuilder builder) {
            builder.Register<ProgressProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterInstance(new ProgressSavePaths(Application.persistentDataPath));
            builder.Register<IProgressSaveFileStore, ProgressSaveFileStore>(Lifetime.Singleton);
            builder.Register<IProgressSaveStorage, ProgressSaveStorage>(Lifetime.Singleton);
            builder.Register<SaveLoadService>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private void BindGameplayServices(IContainerBuilder builder) {
            builder.Register<IConfigService, ConfigService>(Lifetime.Singleton);
            builder.Register<IStockpileService, StockpileService>(Lifetime.Singleton);
            builder.Register<IProductionService, ProductionService>(Lifetime.Singleton);
        }

        private void BindSimulationFactories(IContainerBuilder builder) {
            builder.Register<ITruckFactory, TruckFactory>(Lifetime.Singleton);
            builder.Register<IEnemyFactory, EnemyFactory>(Lifetime.Singleton);
            builder.Register<IExplosionFactory, ExplosionFactory>(Lifetime.Singleton);
            builder.Register<ISimulationViewFactory, SimulationViewFactory>(Lifetime.Singleton);
        }

        private void BindUiServices(IContainerBuilder builder) {
            builder.Register<ProductionUiService>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private void BindEcsContexts(IContainerBuilder builder) {
            builder.RegisterInstance(Contexts.sharedInstance.game);
            builder.RegisterInstance(Contexts.sharedInstance.input);
            builder.RegisterInstance(Contexts.sharedInstance);
        }

        private static void BindSystemFactory(IContainerBuilder builder) {
            builder.Register<ISystemFactory, SystemFactory>(Lifetime.Singleton);
        }
    }
}
