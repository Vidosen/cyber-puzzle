using Minigames.MatrixBreaching.Config;
using UnityEngine;
using Zenject;

namespace Core.Bootstrap
{
    [CreateAssetMenu(fileName = "ConfigInstaller", menuName = "Installers/Config Installer")]
    public class ConfigInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private MatrixBreachingViewConfig _matrixBreachingViewConfig;
        public override void InstallBindings()
        {
            Container.BindInstance(_matrixBreachingViewConfig).AsSingle();
        }
    }
}