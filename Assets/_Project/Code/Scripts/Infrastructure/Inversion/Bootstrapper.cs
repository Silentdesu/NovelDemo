using Novel.Extensions;
using UnityEngine;

namespace Novel.Infrastructure.Inversion
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        private ServiceLocator _container;
        internal ServiceLocator Container => _container.OrNull() ?? (_container = GetComponent<ServiceLocator>());

        private bool _hasInitialized;

        public void BootstrapOnDemand()
        {
            if (_hasInitialized) return;
            _hasInitialized = true;

            Bootstrap();
        }

        protected abstract void Bootstrap();
    }

    [AddComponentMenu("Service Locator/Global")]
    public sealed class GlobalServiceLocator : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal(true);
        }
    }

    [AddComponentMenu("Service Locator/Scene")]
    public sealed class SceneServiceLocator : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
        }
    }
}
