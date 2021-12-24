using UnityEngine;
using Zenject;

namespace Core.Bootstrap
{
    [CreateAssetMenu(fileName = "ConfigInstaller", menuName = "Installers/Config Installer")]
    public class ConfigInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            
        }
    }
}