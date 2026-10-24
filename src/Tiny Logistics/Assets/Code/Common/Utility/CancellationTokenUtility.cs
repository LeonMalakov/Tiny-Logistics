using System.Threading;

namespace Code.Common.Utility {
    public static class CancellationTokenUtility {
        public static void TryCancelAndDispose(this CancellationTokenSource cts) {
            if (!cts.IsCancellationRequested) {
                cts.Cancel();    
            }
            cts.Dispose();
        }
    }
}