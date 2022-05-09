using System.Collections.Generic;
using Minigames.NetworkInjection.Network;
using UnityEngine;
using Zenject;

namespace Minigames.NetworkInjection.Bootstrap
{
    [AddComponentMenu("Installers/E-Network Installer")]
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