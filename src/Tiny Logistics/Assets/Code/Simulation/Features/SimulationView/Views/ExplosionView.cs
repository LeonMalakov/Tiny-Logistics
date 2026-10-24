using Code.Common;
using UnityEngine;

namespace Code.Simulation.Features.SimulationView.Views {
    public sealed class ExplosionView : MonoBehaviour {
        [SerializeField] private LineRenderer _ring;
        [SerializeField] private Color _color = new(1f, 0.5f, 0.1f, 0.9f);

        private float _radius;
        private float _duration;
        private float _age;
        private Vector3 _baseScale;
        private MaterialPropertyBlock _tint;

        public void Validate() {
            if (_ring == null || _ring.sharedMaterial == null || _ring.positionCount < 3) {
                ThrowException.InvalidOperation($"Explosion view '{name}' must have a configured ring.");
            }
        }

        public void Configure(float radius, float duration) {
            Validate();
            _radius = radius;
            _duration = duration;
            _age = 0f;
            _baseScale = _ring.transform.localScale;
            _tint = new MaterialPropertyBlock();
            _ring.transform.localScale = _baseScale * (_radius * 0.1f);
            PrimitiveGeometry.Tint(_ring, _color, _tint);
        }

        private void Update() {
            _age += Time.deltaTime;

            float progress = Mathf.Clamp01(_age / _duration);

            _ring.transform.localScale = _baseScale * (_radius * Mathf.Lerp(0.1f, 1f, progress));

            Color color = _color;

            color.a *= 1f - progress;
            PrimitiveGeometry.Tint(_ring, color, _tint);

            if (_age >= _duration) {
                PrimitiveGeometry.Destroy(gameObject);
            }
        }
        
    }
}
