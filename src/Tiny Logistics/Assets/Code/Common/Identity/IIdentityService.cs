namespace Code.Common.Identity {
    public interface IIdentityService {
        int Next();
        void Reserve(int id);
    }
}