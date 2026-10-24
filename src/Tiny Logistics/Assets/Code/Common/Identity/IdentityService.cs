using System.Collections.Generic;

namespace Code.Common.Identity {
    public class IdentityService : IIdentityService {
        private readonly GameContext _gameContext;
        private readonly HashSet<int> _usedIdsBuffer = new();
        private bool _isIdBufferFilled;
        
        public IdentityService(GameContext gameContext) {
            _gameContext = gameContext;
        }
        
        public int Next() {
            FillIdBuffer();

            int nextId = 1;

            while (_usedIdsBuffer.Contains(nextId)) {
                nextId++;
                
                if (nextId == int.MaxValue) {
                    ThrowException.Default($"Id overflow. Used ids: {_usedIdsBuffer.Count}");
                }
            }
            
            _usedIdsBuffer.Add(nextId);
            
            return nextId;
        }

        public void Reserve(int id) {
            if (id <= 0) {
                return;
            }

            _usedIdsBuffer.Add(id);
        }

        private void FillIdBuffer() {
            if (_isIdBufferFilled) {
                return;
            }

            foreach (var entity in _gameContext.GetGroup(GameMatcher.RuntimeId)) {
                _usedIdsBuffer.Add(entity.RuntimeId);
            }

            _isIdBufferFilled = true;
        }
    }
}