using Code.Infrastructure.States.App;
using UnityEngine;
using VContainer.Unity;

namespace Code.Infrastructure {
    public class AppBootstrapper : IStartable {
        private readonly AppStateMachine _appStateMachine;

        public AppBootstrapper(AppStateMachine appStateMachine) {
            _appStateMachine = appStateMachine;
        }

        public void Start() {
            Application.targetFrameRate = -1;
            _appStateMachine.RegisterStates();
            _appStateMachine.Enter<BootState>();
        }
    }
}