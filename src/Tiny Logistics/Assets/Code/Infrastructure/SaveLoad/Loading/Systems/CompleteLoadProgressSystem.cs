using Code.Infrastructure.SaveLoad.Services;
using Entitas;

namespace Code.Infrastructure.SaveLoad.Loading.Systems {
    public sealed class CompleteLoadProgressSystem : IInitializeSystem {
        private readonly ISaveLoadService _saveLoadService;

        public CompleteLoadProgressSystem(ISaveLoadService saveLoadService) => _saveLoadService = saveLoadService;

        public void Initialize() => _saveLoadService.CompleteLoading();
    }
}
