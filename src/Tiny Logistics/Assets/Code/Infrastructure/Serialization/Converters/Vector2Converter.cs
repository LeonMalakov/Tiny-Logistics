using System;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Infrastructure.Serialization.Converters {
    public class Vector2Converter : JsonConverter<Vector2> {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer) {
            writer.WriteValue($"{value.x.ToString(CultureInfo.InvariantCulture)};{value.y.ToString(CultureInfo.InvariantCulture)}");
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, 
            bool hasExistingValue, JsonSerializer serializer) {
            
            if (reader.Value is not string value) {
                return default;
            }
            
            string[] parts = value.Split(';');
            
            if (parts.Length != 2) {
                return default;
            }
            
            return new Vector2(
                float.Parse(parts[0], CultureInfo.InvariantCulture), 
                float.Parse(parts[1], CultureInfo.InvariantCulture));
        }
    }
}