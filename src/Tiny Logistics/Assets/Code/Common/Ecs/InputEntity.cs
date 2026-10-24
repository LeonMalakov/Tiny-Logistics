using Code.Common.Ecs;

public partial class InputEntity {
    public override string ToString() {
#if UNITY_EDITOR
        return this.Print();
#else
        return string.Empty;
#endif
    }
}
