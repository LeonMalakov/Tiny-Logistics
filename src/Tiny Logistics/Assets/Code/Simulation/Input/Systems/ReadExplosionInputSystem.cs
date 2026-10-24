using System.Collections.Generic;
using Code.Infrastructure.Broker;
using Code.Infrastructure.Configs;
using Code.Simulation.Features.Explosion.Model;
using Entitas;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Code.Simulation.Input.Systems {
    public sealed class ReadExplosionInputSystem : IExecuteSystem {
        private readonly IRequestBroker _broker;
        private readonly IConfigService _configs;
        private readonly List<RaycastResult> _uiHits = new();

        public ReadExplosionInputSystem(IRequestBroker broker, IConfigService configs) {
            _broker = broker;
            _configs = configs;
        }

        public void Execute() {
            Mouse mouse = Mouse.current;

            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) {
                return;
            }

            Vector2 screenPosition = mouse.position.ReadValue();

            if (IsPointerOverUi(screenPosition)) {
                return;
            }

            Camera camera = Camera.main;

            if (camera == null) {
                return;
            }

            Ray ray = camera.ScreenPointToRay(screenPosition);
            Plane ground = new(Vector3.up, _configs.World.FactoryPosition);

            if (ground.Raycast(ray, out float distance)) {
                _broker.Publish(new ExplodeAtRequest(ray.GetPoint(distance)));
            }
        }

        private bool IsPointerOverUi(Vector2 screenPosition) {
            EventSystem eventSystem = EventSystem.current;

            if (eventSystem == null) {
                return false;
            }

            _uiHits.Clear();
            
            eventSystem.RaycastAll(new PointerEventData(eventSystem) {
                position = screenPosition
            }, _uiHits);
            
            return _uiHits.Count > 0;
        }
    }
}