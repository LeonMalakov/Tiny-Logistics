using System;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Configs;
using Code.Infrastructure.Configs.Model;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Editor.Unit.Configs {
    [Category("Unit")]
    [Category("Configuration")]
    public sealed class ConfigServiceTests {
        [Test]
        public void LoadAll_MissingRepository_ThrowsInvalidOperationException() {
            IAssetRepository assets = Substitute.For<IAssetRepository>();
            assets.Configs.Returns((ConfigRepository)null);
            ConfigService service = new(assets);

            Assert.Throws<InvalidOperationException>(() => service.LoadAll().GetAwaiter().GetResult());
        }
    }
}
