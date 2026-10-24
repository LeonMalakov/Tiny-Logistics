using System;
using Code.Common.Identity;
using Code.Common.Time;
using Code.Infrastructure.Broker;
using Code.Infrastructure.Configs;
using Code.Infrastructure.Configs.Model;
using Code.Infrastructure.Ecs.Systems;
using Code.Infrastructure.SaveLoad.Loading;
using Code.Infrastructure.SaveLoad.Saving;
using Code.Infrastructure.SaveLoad.Services;
using Code.Infrastructure.SaveLoad.Storage;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.Enemies.Factories;
using Code.Simulation.Features.Explosion.Factories;
using Code.Simulation.Features.Production.Services;
using Code.Simulation.Features.ProductionUi.Services;
using Code.Simulation.Features.ProductionUi.UiServices;
using Code.Simulation.Features.SimulationView.Factories;
using Code.Simulation.Features.Stockpile.Services;
using Code.Simulation.Features.Trucks.Factories;
using Cysharp.Threading.Tasks;
using NSubstitute;
using UnityEngine;
using VContainer;

namespace Tests.Editor.Support {
    internal sealed class GameTestContext : IDisposable {
        public readonly BalanceConfig Balance;
        public readonly WorldConfig World;
        public readonly IConfigService Configs;
        public ViewConfig View => Configs.View;
        public readonly GameContext Game = new();
        public readonly TestTime Time = new();
        public readonly UiDispatcher Dispatcher = new();
        public readonly RequestBroker Broker = new();
        public readonly StockpileService Stockpile;
        public readonly TestSaveFileStore SaveFiles = new();
        public readonly ProgressProvider Progress = new();
        public readonly IProgressSaveStorage SaveStorage;
        public readonly IObjectResolver Container;
        private readonly ISimulationViewFactory _views;

        public ISystemFactory Systems => Container.Resolve<ISystemFactory>();
        public IProductionService Production => Container.Resolve<IProductionService>();
        public IProductionUiService Ui => Container.Resolve<IProductionUiService>();
        public ITruckFactory Trucks => Container.Resolve<ITruckFactory>();
        public IEnemyFactory Enemies => Container.Resolve<IEnemyFactory>();
        public IExplosionFactory Explosions => Container.Resolve<IExplosionFactory>();
        public ISaveLoadService Saves => Container.Resolve<ISaveLoadService>();

        public GameTestContext(bool includeViews = false) {
            Balance = ScriptableObject.CreateInstance<BalanceConfig>();
            World = ScriptableObject.CreateInstance<WorldConfig>();
            ViewConfig view = includeViews
                ? UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<ViewConfig>("Assets/Configs/ViewConfig.asset"))
                : null;
            Configs = Substitute.For<IConfigService>();
            Configs.Balance.Returns(Balance);
            Configs.World.Returns(World);
            Configs.View.Returns(view);
            Configs.LoadAll().Returns(UniTask.CompletedTask);
            Stockpile = new StockpileService(Dispatcher);
            SaveStorage = new ProgressSaveStorage(new ProgressSavePaths("Tests"), SaveFiles);
            ContainerBuilder builder = new();
            builder.RegisterInstance(Game);
            builder.RegisterInstance(Time).As<ITimeService>();
            builder.RegisterInstance(Configs).As<IConfigService>();
            builder.RegisterInstance(Dispatcher).As<IUiDispatcher>();
            builder.RegisterInstance(Broker).As<IRequestBroker>();
            builder.RegisterInstance(Stockpile).As<IStockpileService>();
            builder.RegisterInstance(Progress).As<IProgressProvider>();
            builder.RegisterInstance(SaveStorage);
            builder.Register<ISaveLoadService, SaveLoadService>(Lifetime.Singleton);
            builder.Register<IIdentityService, IdentityService>(Lifetime.Singleton);
            builder.Register<ISystemFactory, SystemFactory>(Lifetime.Singleton);
            builder.Register<ITruckFactory, TruckFactory>(Lifetime.Singleton);
            builder.Register<IEnemyFactory, EnemyFactory>(Lifetime.Singleton);
            builder.Register<IExplosionFactory, ExplosionFactory>(Lifetime.Singleton);
            if (includeViews) {
                builder.Register<ISimulationViewFactory, SimulationViewFactory>(Lifetime.Singleton);
            }

            builder.Register<IProductionService, ProductionService>(Lifetime.Singleton);
            builder.Register<ProductionUiService>(Lifetime.Singleton).AsImplementedInterfaces();
            Container = builder.Build();
            _views = includeViews ? Container.Resolve<ISimulationViewFactory>() : null;
            Production.Reset();
            Dispatcher.Flush();
        }

        public GameEntity CreateFactory() {
            GameEntity factory = Game.CreateEntity();
            factory.isFactory = true;
            factory.AddRuntimeId(Container.Resolve<IIdentityService>().Next());
            factory.AddWorldPosition(World.FactoryPosition);
            factory.AddCraftProgress(0f);
            factory.AddCraftDuration(Production.CraftDuration);
            factory.AddHealth(Balance.FactoryHealth);
            factory.AddMaxHealth(Balance.FactoryHealth);

            return factory;
        }

        public void SaveProgress() {
            Game.CreateEntity().isSaveProgressRequest = true;
            Systems.Create<SaveProgressFeature>().Execute();
        }

        public bool RestoreProgress() {
            if (Progress.ProgressData == null)
                return false;

            Systems.Create<LoadProgressFeature>().Initialize();

            return true;
        }

        public void Dispose() {
            _views?.ClearWorld();
            Game.DestroyAllEntities();
            Container.Dispose();
            UnityEngine.Object.DestroyImmediate(Balance);
            UnityEngine.Object.DestroyImmediate(World);
            UnityEngine.Object.DestroyImmediate(View);
        }

        internal sealed class TestTime : ITimeService {
            public float DeltaTime { get; set; } = 0.25f;
        }
    }
}
