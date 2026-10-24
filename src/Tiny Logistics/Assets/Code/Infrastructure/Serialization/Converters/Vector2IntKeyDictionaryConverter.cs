using System;
using System.Collections.Generic;
using Code.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Infrastructure.Serialization.Converters {
    public class Vector2IntKeyDictionaryConverter<TValue> : JsonConverter<Dictionary<Vector2Int, TValue>> {
        public override void WriteJson(JsonWriter writer, Dictionary<Vector2Int, TValue> value, JsonSerializer serializer) {
            writer.WriteStartObject();
            
            foreach (KeyValuePair<Vector2Int, TValue> kvp in value) {
                string key = $"{kvp.Key.x};{kvp.Key.y}";
                writer.WritePropertyName(key);
                serializer.Serialize(writer, kvp.Value);
            }

            writer.WriteEndObject();
        }

        public override Dictionary<Vector2Int, TValue> ReadJson(JsonReader reader, Type objectType, 
            Dictionary<Vector2Int, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer) {
            
            JObject obj = JObject.Load(reader);
            Dictionary<Vector2Int, TValue> dict = new Dictionary<Vector2Int, TValue>();

            foreach (JProperty prop in obj.Properties()) {
                string[] parts = prop.Name.Split(';');
                if (parts.Length != 2 ||
                    !int.TryParse(parts[0], out int x) ||
                    !int.TryParse(parts[1], out int y))
                    return ThrowException.JsonSerialization<Dictionary<Vector2Int, TValue>>($"Invalid Vector2Int key: {prop.Name}");

                Vector2Int key = new Vector2Int(x, y);
                TValue value = prop.Value.ToObject<TValue>(serializer);
                dict[key] = value;
            }

            return dict;
        }
    }
}