using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Configs;
using Code.Infrastructure.Configs.Model;
using NSubstitute;
using NUnit.Framework;
using Tests.Editor.Support;
using UnityEditor;

namespace Tests.Editor.Integration.Infrastructure {
    [Category("Integration")]
    [Category("Configuration")]
    public sealed class ConfigLoadingTests {
        [Test]
        public void LoadAll_UsesConfigsFromAssetRepository() {
            ConfigRepository repository = AssetDatabase.LoadAssetAtPath<ConfigRepository>(TestAssetPaths.ConfigRepository);
            IAssetRepository assets = Substitute.For<IAssetRepository>();
            assets.Configs.Returns(repository);
            ConfigService service = new(assets);

            service.LoadAll().GetAwaiter().GetResult();

            Assert.That(service.Balance, Is.SameAs(repository.Balance));
            Assert.That(service.World, Is.SameAs(repository.World));
            Assert.That(service.View, Is.SameAs(repository.View));
        }
    }
}
