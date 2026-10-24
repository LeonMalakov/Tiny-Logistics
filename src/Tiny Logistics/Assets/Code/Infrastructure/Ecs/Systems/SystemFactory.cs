using Entitas;
using VContainer;

namespace Code.Infrastructure.Ecs.Systems {
    public class SystemFactory : ISystemFactory {
        private readonly IObjectResolver _resolver;

        public SystemFactory(IObjectResolver resolver) {
            _resolver = resolver;
        }
        
        public T Create<T>() where T : ISystem {
            // TODO: кодогенератор на регистрацию Transient типов и спавн через Resolve.
            
            var constructor = typeof(T).GetConstructors()[0];
            var parameters = constructor.GetParameters();
            var parameterInstances = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++) {
                parameterInstances[i] = _resolver.Resolve(parameters[i].ParameterType);
            }

            T instance = (T)constructor.Invoke(parameterInstances);
            return instance;
        }
    }
}