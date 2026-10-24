using Code.Infrastructure.Broker;
using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Explosion.Factories;
using Code.Simulation.Features.Explosion.Model;

namespace Code.Simulation.Features.Explosion.Systems {
    public sealed class CreateExplosionSystem : BrokerRequestHandlerSystem<ExplodeAtRequest> {
        private readonly IExplosionFactory _explosions;

        public CreateExplosionSystem(IRequestBroker broker, IExplosionFactory explosions) : base(broker) {
            _explosions = explosions;
        }

        protected override void Handle(ExplodeAtRequest request) => _explosions.Create(request.Position);
    }
}
