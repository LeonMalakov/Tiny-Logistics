using System.Collections.Generic;

namespace Code.Infrastructure.Ecs {
    public static class GameEntityBuffer {
        public static readonly List<GameEntity> A = new List<GameEntity>(1024);
        
        public static readonly List<GameEntity> ExternalA = new List<GameEntity>(1024);
    }
}