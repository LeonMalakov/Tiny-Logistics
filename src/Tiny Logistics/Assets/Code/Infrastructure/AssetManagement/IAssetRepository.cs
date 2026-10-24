using Code.Infrastructure.Configs.Model;

namespace Code.Infrastructure.AssetManagement {
    public interface IAssetRepository {
        public ConfigRepository Configs { get; }
    }
}