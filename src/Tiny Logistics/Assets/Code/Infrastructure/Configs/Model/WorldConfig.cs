using Code.Common;
using UnityEngine;

namespace Code.Infrastructure.Configs.Model {
    [CreateAssetMenu(menuName = "Configs/WorldConfig")]
    public sealed class WorldConfig : ScriptableObject {
        [Tooltip("Map rectangle on the XZ plane; its edges are included.")]
        public Rect MapBounds = new(-10f, -10f, 20f, 20f);
        public Vector3 FactoryPosition = new(-5f, 0f, 0f);
        public Vector3 TruckSpawnOffset = new(1.5f, 0f, 0f);
        [Tooltip("Enemy spawn rectangle below the truck route, in XZ coordinates.")]
        public Rect EnemySpawnArea = new(-3f, -9f, 11f, 1f);

        public void Validate() {
            ConfigValidation.Rectangle(MapBounds, nameof(MapBounds));
            ConfigValidation.Rectangle(EnemySpawnArea, nameof(EnemySpawnArea));
            
            ValidatePosition(FactoryPosition, nameof(FactoryPosition));
            ValidatePosition(TruckSpawnOffset, nameof(TruckSpawnOffset));
            
            if (!ConfigValidation.Contains(MapBounds, FactoryPosition) ||
                !ConfigValidation.Contains(MapBounds, FactoryPosition + TruckSpawnOffset)) {
                ThrowException.InvalidArgument("Factory and truck spawn must be inside the map.");
            }
            
            if (EnemySpawnArea.xMin < MapBounds.xMin || EnemySpawnArea.xMax > MapBounds.xMax ||
                EnemySpawnArea.yMin < MapBounds.yMin || EnemySpawnArea.yMax > MapBounds.yMax ||
                EnemySpawnArea.yMax >= FactoryPosition.z + TruckSpawnOffset.z) {
                ThrowException.InvalidArgument("Enemy spawn area must be inside the map and below the truck route.");
            }
        }
        private static void ValidatePosition(Vector3 position, string name) {
            ConfigValidation.Finite(position.x, name);
            ConfigValidation.Finite(position.y, name);
            ConfigValidation.Finite(position.z, name);
        }
    }
}
