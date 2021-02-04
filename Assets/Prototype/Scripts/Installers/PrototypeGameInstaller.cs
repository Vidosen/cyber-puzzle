using Prototype.Scripts.Data.Singals;
using Prototype.Scripts.Services;
using UnityEngine;
using Zenject;

namespace Prototype
{
    [CreateAssetMenu(fileName = "PrototypeGameInstaller", menuName = "Installers/PrototypeGameInstaller")]
    public class PrototypeGameInstaller : ScriptableObjectInstaller<PrototypeGameInstaller>
    {
        public override void InstallBindings()
        {
            InstallServices();
        }

        private void InstallServices()
        {
            Container
                .BindInterfacesAndSelfTo<GameProcessService>()
                .AsSingle()
                .NonLazy();


        }
        private void InstallSignals()
        {
            Container.DeclareSignal<MatrixOperationsSignals.ColumnsSwaped>();
            Container.DeclareSignal<MatrixOperationsSignals.RowsSwaped>();
            Container.DeclareSignal<MatrixOperationsSignals.ColumnsSwapRequest>();
            Container.DeclareSignal<MatrixOperationsSignals.RowsSwapRequest>();
        }
    }
}