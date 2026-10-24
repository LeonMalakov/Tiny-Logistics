using Code.Common;
using Code.Infrastructure.Configs.Model;
using Code.Infrastructure.Ecs;
using UnityEngine;

namespace Code.Simulation.Features.SimulationView.Views {
    public sealed class SimulationEntityView : EntityView {
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private SpriteRenderer _healthFill;
        [SerializeField] private Transform _healthBar;

        private Vector3 _healthScale;
        private Vector3 _healthPosition;
        private Vector3 _healthDirection;
        private float _healthLeftEdge;
        private Color _bodyColor;
        private ViewConfig _config;

        public void Validate() {
            if (_body == null || _body.sprite == null || _body.sharedMaterial == null ||
                _healthFill == null || _healthFill.sprite == null || _healthFill.sharedMaterial == null ||
                _healthBar == null) {
                ThrowException.InvalidOperation(
                    $"Entity view '{name}' must have a sprite renderer and health bar references.");
            }
        }

        public void Configure(ViewConfig config) {
            Validate();
            _healthScale = _healthFill.transform.localScale;
            _healthPosition = _healthFill.transform.localPosition;
            _healthDirection = _healthFill.transform.localRotation * Vector3.right;
            _healthLeftEdge = _healthFill.sprite.bounds.min.x * _healthScale.x;
            _bodyColor = _body.color;
            _config = config;
        }

        public void Refresh(float time) {
            transform.position = Entity.WorldPosition;

            Color color = Entity.isDead 
                ? _config.DeadColor 
                : _bodyColor;

            if (!Entity.isDead && Entity.isBurning) {
                color = Color.Lerp(color, 
                    _config.BurningColor,
                    0.5f + 0.5f * Mathf.Sin(time * _config.BurningPulseSpeed * Mathf.PI * 2f));
            }

            _body.color = color;

            bool hasHealth = Entity.hasHealth && Entity.hasMaxHealth && Entity.MaxHealth > 0f;

            _healthBar.gameObject.SetActive(hasHealth);

            if (!hasHealth) {
                return;
            }

            float fraction = Mathf.Clamp01(Entity.Health / Entity.MaxHealth);
            Vector3 size = _healthScale;

            size.x *= fraction;
            _healthFill.transform.localScale = size;
            _healthFill.transform.localPosition = _healthPosition + _healthDirection * (_healthLeftEdge * (1f - fraction));
            _healthFill.color = Color.Lerp(new Color(0.9f, 0.18f, 0.15f), new Color(0.3f, 0.9f, 0.55f), fraction);
        }
    }
}
