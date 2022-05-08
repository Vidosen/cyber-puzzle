using System.Collections.Generic;
using Minigames.Network_Injection.Network;
using Zenject;

namespace Minigames.Network_Injection.Bootstrap
{
    public class ENetworkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ENetworkInventory>().AsSingle();
            Container
                .BindFactory<string, List<ENetworkModel.Node>, List<ENetworkModel.Connection>, ENetworkModel,
                    ENetworkFactory>()
                .FromNew();
        }
    }
}