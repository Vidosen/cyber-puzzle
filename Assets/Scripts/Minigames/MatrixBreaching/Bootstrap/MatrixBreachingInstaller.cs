using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Providers;
using Zenject;

namespace Minigames.MatrixBreaching.Bootstrap
{
    public class MatrixBreachingInstaller : MonoInstaller
    {
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
        }
    }
}