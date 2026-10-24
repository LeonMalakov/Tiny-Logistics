using System.Collections.Generic;
using Code.Infrastructure.Serialization.Converters;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Infrastructure.Serialization {
    public static class CustomConverters {
        public static readonly JsonConverter[] Converters = {
            new Vector2IntConverter(),
            new Vector2Converter(),
            new Vector2IntKeyDictionaryConverter<HashSet<Vector2Int>>(),
            new Vector2IntKeyDictionaryConverter<long>(),
            new Vector2IntKeyDictionaryConverter<double>(),
            new Vector2IntKeyDictionaryConverter<int>(),
            new Vector2IntKeyDictionaryConverter<Dictionary<Vector2Int, long>>()
        };
    }
}