using Minigames.MatrixBreaching.Core;
using Minigames.MatrixBreaching.Core.Rules;
using Minigames.MatrixBreaching.Matrix;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Operations;
using Minigames.MatrixBreaching.Matrix.Operations.Commands;
using Minigames.MatrixBreaching.Matrix.Operations.ViewProcessors;
using Minigames.MatrixBreaching.Matrix.Providers;
using Minigames.MatrixBreaching.Matrix.Rules;
using Minigames.MatrixBreaching.Matrix.Signals;
using Minigames.MatrixBreaching.Matrix.Views;
using Minigames.MatrixBreaching.Vulnerabilities;
using Minigames.MatrixBreaching.Vulnerabilities.Models;
using Minigames.MatrixBreaching.Vulnerabilities.Rules;
using Minigames.MatrixBreaching.Vulnerabilities.Services;
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
            Container.Bind<ICellProvider>().To<RandomCellProvider>().AsSingle()
                .OnInstantiated((context, obj) =>
                {
                    if (obj is RandomCellProvider randomValueCellProvider)
                        randomValueCellProvider.SetRandomSeed(RandomValueMatrxSeed);
                });
            
            Container.BindInstance(_matrixPresenter);
            Container.Bind<SwapCommandsProcessor>().AsSingle();
            Container.BindInterfacesTo<VerticalSwapViewProcessor>().AsSingle();
            Container.BindInterfacesTo<HorizontalSwapViewProcessor>().AsSingle();
            
            Container.Bind<ScrollCommandsProcessor>().AsSingle();
            Container.BindInterfacesTo<ScrollViewProcessor>().AsSingle();

            Container.BindFactory<OperationType, RowType, IMatrixCommand, IMatrixCommand.Factory>()
                .FromFactory<OperationCommandFactory>();

            Container.DeclareSignalWithInterfaces<MatrixOperationsSignals.SwapOperationOccured>();
            Container.DeclareSignalWithInterfaces<MatrixOperationsSignals.ScrollOperationOccured>();
            Container.DeclareSignalWithInterfaces<MatrixOperationsSignals.PostOperationOccured>().OptionalSubscriber();
            Container.DeclareSignal<MatrixOperationsSignals.OperationApplied>();
            Container.DeclareSignal<MatrixSignals.CellDisposed>().OptionalSubscriber();
            Container.DeclareSignal<MatrixSignals.CellMoved>().OptionalSubscriber();

            Container.BindFactory<string, VulnerabilityModel, VulnerabiltyFactory>().FromNew();
            Container.Bind<VulnerabiltyInventory>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<VulnerabilityService>().AsSingle()
                .OnInstantiated((context, obj) =>
            {
                if (obj is VulnerabilityService vulnerabilityService)
                    vulnerabilityService.SetRandomSeed(RandomValueMatrxSeed);
            });
            Container.Bind<MatrixBreachingModel>().AsSingle();
            
            Container.BindInterfacesTo<VulnerabilitiesManagementRule>().AsSingle();
            Container.BindInterfacesTo<CheckVulnerabilitiesRule>().AsSingle();
            
            Container.BindInterfacesTo<PostOperationGlitchRule>().AsSingle()
                .OnInstantiated((context, obj) =>
                {
                    if (obj is PostOperationGlitchRule postOperationShuffle)
                        postOperationShuffle.SetRandomSeed(RandomValueMatrxSeed);
                });
            Container.BindInterfacesTo<PostOperationShuffleRule>().AsSingle()
                .OnInstantiated((context, obj) =>
            {
                if (obj is PostOperationShuffleRule postOperationShuffle)
                    postOperationShuffle.SetRandomSeed(RandomValueMatrxSeed);
            });
            Container.BindInterfacesTo<PostOperationLockRule>().AsSingle();
            Container.BindInterfacesTo<AddProgressForRemovedVulnerabilityRule>().AsSingle();
            Container.BindInterfacesTo<DamagePerIntervalToVirusRule>().AsSingle();
        }
    }
}