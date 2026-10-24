using System.Collections.Generic;
using Code.Simulation.Features.Stockpile.Model;
using Newtonsoft.Json;

namespace Code.Infrastructure.SaveLoad.Model {
    public sealed class ProgressData {
        public const int CurrentVersion = 1;

        [JsonProperty("Version", Required = Required.Always)] public int Version;
        [JsonProperty("ProductionUpgradeLevel", Required = Required.Always)] public int ProductionUpgradeLevel;
        [JsonProperty("Stockpile", Required = Required.Always)] public Dictionary<ItemId, long> Stockpile = new();
        [JsonProperty("Entities", Required = Required.Always)] public List<EntitySnapshotData> Entities = new();
    }
}
