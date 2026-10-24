using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Infrastructure.Serialization.Converters {
    public class Vector2IntConverter : JsonConverter<Vector2Int> {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer) {
            writer.WriteValue($"{value.x};{value.y}");
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue,
            bool hasExistingValue, JsonSerializer serializer) {
            if (reader.Value is not string value) {
                return default;
            }

            string[] parts = value.Split(';');

            if (parts.Length != 2) {
                return default;
            }

            return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }
}