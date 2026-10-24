using Code.Infrastructure.Configs.Model;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement {
    public class AssetRepository : MonoBehaviour, IAssetRepository {
        public ConfigRepository Configs;
        
        ConfigRepository IAssetRepository.Configs => Configs;
    }
}