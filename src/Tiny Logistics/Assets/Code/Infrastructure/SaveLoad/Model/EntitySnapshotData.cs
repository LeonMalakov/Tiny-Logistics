using Newtonsoft.Json;
using UnityEngine;

namespace Code.Infrastructure.SaveLoad.Model {
    public enum SavedEntityKind {
        Factory = 1,
        Truck = 2,
        Enemy = 3
    }

    public sealed class EntitySnapshotData {
        [JsonProperty("Kind", Required = Required.Always)] public SavedEntityKind Kind;
        [JsonProperty("Position", Required = Required.Always)] public SavedVector3 Position;
        [JsonProperty("Health", Required = Required.Always)] public float Health;
        [JsonProperty("MaxHealth", Required = Required.Always)] public float MaxHealth;
        [JsonProperty("Dead")] public bool Dead;
        [JsonProperty("Velocity")] public SavedVector3? Velocity;
        [JsonProperty("EnemySpeed")] public float? EnemySpeed;
        [JsonProperty("CraftProgress")] public float? CraftProgress;
        [JsonProperty("BurningTimeLeft")] public float? BurningTimeLeft;
        [JsonProperty("BurnDamagePerSecond")] public float? BurnDamagePerSecond;
        [JsonProperty("SelfDestructTimeLeft")] public float? SelfDestructTimeLeft;
    }

    public struct SavedVector3 {
        [JsonProperty("X", Required = Required.Always)] public float X;
        [JsonProperty("Y", Required = Required.Always)] public float Y;
        [JsonProperty("Z", Required = Required.Always)] public float Z;

        public SavedVector3(Vector3 value) {
            X = value.x;
            Y = value.y;
            Z = value.z;
        }

        public Vector3 ToVector3() => new(X, Y, Z);
    }
}
