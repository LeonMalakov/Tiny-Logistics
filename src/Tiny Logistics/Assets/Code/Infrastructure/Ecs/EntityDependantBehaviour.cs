using UnityEngine;

namespace Code.Infrastructure.Ecs {
    public abstract class EntityDependantBehaviour : MonoBehaviour {
        public EntityView View;

        public GameEntity Entity => View.Entity;
        
#if UNITY_EDITOR
        private void OnValidate() {
            if (View == null) {
                View = GetComponentInParent<EntityView>(true);
            }
        }
#endif
    }
}