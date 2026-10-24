using System.Threading;

namespace Code.Infrastructure.States {
    public static class StateIndexer<T> {
        // ReSharper disable once StaticMemberInGenericType
        public static readonly int Index;
        
        static StateIndexer() {
            Index = StateIndexer.CreateIndex();
        }
    }
    
    public static class StateIndexer {
        private static int LastIndex = 0;
        
        public static int CreateIndex() {
            int index = Interlocked.Increment(ref LastIndex);
            return index;
        }
    }
}