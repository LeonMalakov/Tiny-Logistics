using UnityEngine;

namespace Code.Infrastructure.Configs.Model {
    [CreateAssetMenu(menuName = "Configs/ConfigRepository")]
    public class ConfigRepository : ScriptableObject {
        public BalanceConfig Balance;
        public WorldConfig World;
        public ViewConfig View;
    }
}
