using Minigames.MatrixBreaching.Core.Services;
using Zenject;

namespace Core.Bootstrap
{
    public class MainInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.BindInterfacesAndSelfTo<MatrixBreachingService>().AsSingle();
        }
    }
}