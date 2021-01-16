using UnityEngine;
using Zenject;

namespace Prototype
{
    [CreateAssetMenu(fileName = "PrototypeGameInstaller", menuName = "Installers/PrototypeGameInstaller")]
    public class PrototypeGameInstaller : ScriptableObjectInstaller<PrototypeGameInstaller>
    {
        public override void InstallBindings()
        {
        
        }
    }
}