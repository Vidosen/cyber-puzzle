using Prototype.Scripts.Data;
using Prototype.Scripts.Data.Singals;
using Prototype.Scripts.Services;
using Prototype.Scripts.Views;
using UnityEngine;
using Zenject;

namespace Prototype.Scripts.Installers
{
    [CreateAssetMenu(fileName = "PrototypeGameInstaller", menuName = "Installers/PrototypeGameInstaller")]
    public class PrototypeGameInstaller : ScriptableObjectInstaller<PrototypeGameInstaller>
    {
        [SerializeField] private GameMatrix gameMatrixPrefab;
        [SerializeField, Space] private Combination combinationPrefab;
        public override void InstallBindings()
        {
            InstallPrefabs();
            
            InstallServices();
        }

        private void InstallServices()
        {
            Container
                .BindInterfacesAndSelfTo<GameService>()
                .AsSingle()
                .NonLazy();


        }

        private void InstallPrefabs()
        {
            Container.Bind<GameMatrix>().FromInstance(gameMatrixPrefab).AsSingle();
            Container.Bind<Combination>().FromInstance(combinationPrefab).AsSingle();
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