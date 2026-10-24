#if UNITY_EDITOR

using System.Collections.Generic;
using Code.Infrastructure.Ecs;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class EditorEcsUtility {
    public static List<EntityComponentRegistrar> GetRegistrars(EntityView entityView) {
        void Iteration(Transform t, List<EntityComponentRegistrar> list) {
            if (t.TryGetComponent<EntityView>(out var view) && view != entityView) {
                return;
            }

            foreach (var registrar in t.GetComponents<EntityComponentRegistrar>()) {
                list.Add(registrar);
            }

            foreach (Transform child in t) {
                Iteration(child, list);
            }
        }

        var list = new List<EntityComponentRegistrar>();
        Iteration(entityView.transform, list);
        return list;
    }
}
#endif