using UnityEngine;
using Zenject;

namespace Prototype.Scripts.Installers
{
    public class RectTransformInstaller : MonoInstaller
    {
        public string InjectId;
        public override void InstallBindings()
        {
            Container.Bind<RectTransform>().WithId(InjectId).FromInstance(transform as RectTransform);
        }
    }
}