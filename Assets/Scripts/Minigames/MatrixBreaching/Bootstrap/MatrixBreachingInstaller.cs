using Minigames.MatrixBreaching.Matrix;
using Minigames.MatrixBreaching.Matrix.Commands;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Providers;
using Minigames.MatrixBreaching.Matrix.Signals;
using Minigames.MatrixBreaching.Views;
using UnityEngine;
using Zenject;

namespace Minigames.MatrixBreaching.Bootstrap
{
    public class MatrixBreachingInstaller : MonoInstaller
    {
        [SerializeField] private GuardMatrixPresenter _matrixPresenter;
        public int RandomValueMatrxSeed;

        public override void InstallBindings()
        {
            Container.Bind<GuardMatrix>().ToSelf().AsSingle().NonLazy();
            Container.Bind<ICellProvider>().To<RandomValueCellProvider>().AsSingle()
                .OnInstantiated((context, obj) =>
                {
                    if (obj is RandomValueCellProvider randomValueCellProvider)
                        randomValueCellProvider.SetRandomSeed(RandomValueMatrxSeed);
                });
            
            Container.BindInstance(_matrixPresenter);
            Container.Bind<SwapCommandsProcessor>().ToSelf().AsSingle();
            Container.BindInterfacesTo<VerticalSwapViewProcessor>().AsSingle();
            Container.BindInterfacesTo<HorizontalSwapViewProcessor>().AsSingle();
            
            Container.Bind<ScrollCommandsProcessor>().ToSelf().AsSingle();
            Container.BindInterfacesTo<ScrollViewProcessor>().AsSingle();

            Container.BindFactory<OperationType, RowType, IMatrixCommand, IMatrixCommand.Factory>()
                .FromFactory<OperationCommandFactory>();

            Container.DeclareSignal<MatrixOperationsSignals.SwapOperationOccured>();
            Container.DeclareSignal<MatrixOperationsSignals.ScrollOperationOccured>();
        }
    }
}