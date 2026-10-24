using Cysharp.Threading.Tasks;
using Code.Infrastructure.Configs.Model;

namespace Code.Infrastructure.Configs {
    public interface IConfigService {
        BalanceConfig Balance { get; }
        WorldConfig World { get; }
        ViewConfig View { get; }
        UniTask LoadAll();
    }
}
