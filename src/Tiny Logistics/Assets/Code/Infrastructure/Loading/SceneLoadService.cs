using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.Loading {
    public class SceneLoadService : ISceneLoadService {
        public async UniTask Load(string sceneName, bool forceReload = false) {
            if (!forceReload && SceneManager.GetActiveScene().name == sceneName) {
                return;
            }

            await SceneManager
                .LoadSceneAsync(sceneName, LoadSceneMode.Single)
                .ToUniTask();
        }
    }
}
