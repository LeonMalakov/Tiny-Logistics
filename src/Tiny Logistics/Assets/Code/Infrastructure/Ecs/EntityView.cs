using System;
using Code.Common;
using UnityEngine;

namespace Code.Infrastructure.Ecs {
    public class EntityView : MonoBehaviour {
        public EntityComponentRegistrar[] Registrars = Array.Empty<EntityComponentRegistrar>();
        
        private GameEntity _entity;
        
        public GameEntity Entity => _entity;

        public void SetEntity(GameEntity entity) {
            if (_entity != null || entity == null || !entity.isEnabled || entity.hasView) {
                ThrowException.InvalidOperation(
                    "EntityView requires an enabled entity without another view, and must be released before rebinding.");
            }

            _entity = entity;
            _entity.AddView(this);
            _entity.Retain(this);
            
            foreach (var registrar in Registrars ?? Array.Empty<EntityComponentRegistrar>()) {
                registrar.RegisterComponents();
            }
        }

        public void ReleaseEntity() {
            if (_entity == null) {
                return;
            }
            
            GameEntity entity = _entity;
            
            if (entity.isEnabled) {
                foreach (var registrar in Registrars ?? Array.Empty<EntityComponentRegistrar>()) {
                    registrar.UnregisterComponents();
                }

                if (entity.hasView && entity.View == this) {
                    entity.RemoveView();
                }
            }
            
            _entity = null;
            entity.Release(this);
        }

        protected virtual void OnDestroy() => ReleaseEntity();
        
#if UNITY_EDITOR
        public void UpdateRegistrarMap() {
            Registrars = EditorEcsUtility.GetRegistrars(this).ToArray();
        }
        
        private void OnValidate() => UpdateRegistrarMap();
#endif
    }
}
