using Code.Common.Ecs;

public partial class GameEntity {
    public override string ToString() {
#if UNITY_EDITOR
        return this.Print();
#else
        return string.Empty;
#endif
    }
}
