using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

namespace Code.Infrastructure.Serialization {
    public static class JsonSerialization {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.None,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented, // TODO: Remove in production
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            
            Converters = CustomConverters.Converters
                .ToArray()
        };

        static JsonSerialization() {
            // ReSharper disable once ObjectCreationAsStatement
            AotHelper.Ensure(() => new ReferenceConverter(typeof(Dummy)));
        }

        public static string Serialize<T>(T obj) {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public static T Deserialize<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        private class Dummy { }
    }
}