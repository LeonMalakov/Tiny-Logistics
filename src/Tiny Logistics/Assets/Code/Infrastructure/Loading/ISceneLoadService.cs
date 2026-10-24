using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Loading {
    public interface ISceneLoadService {
        UniTask Load(string sceneName, bool forceReload = false);
    }
}
