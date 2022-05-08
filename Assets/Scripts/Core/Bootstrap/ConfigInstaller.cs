using Minigames.MatrixBreaching.Config;
using Minigames.Network_Injection.Configs;
using UnityEngine;
using Zenject;

namespace Core.Bootstrap
{
    [CreateAssetMenu(fileName = "ConfigInstaller", menuName = "Installers/Config Installer")]
    public class ConfigInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private MatrixBreachingViewConfig _matrixBreachingViewConfig;
        [SerializeField] private MatrixBreachingGeneralConfig _matrixBreachingGeneralConfig;
        [SerializeField] private ENetworkSetupConfig _eNetworkSetupConfig;
        public override void InstallBindings()
        {
            Container.BindInstance(_matrixBreachingViewConfig).AsSingle();
            Container.BindInstance(_matrixBreachingGeneralConfig).AsSingle();
            Container.BindInstance(_eNetworkSetupConfig).AsSingle();
        }
    }
}