using Code.Common;
using UnityEngine;

namespace Code.Simulation.Features.SimulationView.Views {
    public sealed class WorldView : MonoBehaviour {
        [SerializeField] private Transform _entityRoot;
        [SerializeField] private Transform _effectRoot;

        public Transform EntityRoot => _entityRoot;
        public Transform EffectRoot => _effectRoot;

        public void Validate() {
            if (_entityRoot == null || _effectRoot == null) {
                ThrowException.InvalidOperation(
                    $"World view '{name}' must have entity and effect root references.");
            }
        }
    }
}
