using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Providers;
using Minigames.MatrixBreaching.Matrix.Signals;
using NUnit.Framework;
using UnityEngine;
using Utils;
using Zenject;

namespace Editor.Tests
{
    [TestFixture]
    public class ProtectMatrixTests : ZenjectUnitTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            SignalBusInstaller.Install(Container);
            Container.DeclareSignalWithInterfaces<MatrixOperationsSignals.SwapOperationOccured>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<MatrixOperationsSignals.ScrollOperationOccured>().OptionalSubscriber();
            Container.DeclareSignal<MatrixSignals.CellDisposed>().OptionalSubscriber();
            Container.Bind<GuardMatrix>().ToSelf().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MockCellProvider>().AsSingle().NonLazy();
        }

        [Test]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        public void Initialized_Matrix_Is_Valid_Test(int h_size, int v_size)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(SimpleFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<GuardMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            AssertMatrixValid(protectMatrix, h_size, v_size);
            protectMatrix.Log();
        }
        
        [Test]
        [TestCase(3, 3, 5, 5)]
        [TestCase(3, 4, 4, 4)]
        [TestCase(4, 3, 8, 3)]
        [TestCase(4, 4, 3, 4)]
        [TestCase(5, 5, 3, 3)]
        public void ReInitialized_Matrix_Is_Valid_Test(int h_size, int v_size, int new_h_siz, int new_v_size)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(SimpleFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<GuardMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            AssertMatrixValid(protectMatrix, h_size, v_size);
            protectMatrix.Log();
            
            protectMatrix.Dispose();
            
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(RepeatFillCellsFunc);
            }
            protectMatrix.Initialize(new_h_siz, new_v_size);
            AssertMatrixValid(protectMatrix, new_h_siz, new_v_size);
            protectMatrix.Log();
        }

        
        
        [Test]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        public void Get_Horizontal_Cells_Test(int h_size, int v_size)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(SimpleFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<GuardMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            protectMatrix.Log();
            for (int verticalId = 0; verticalId < v_size; verticalId++)
            {
                var horizontalCells = protectMatrix.GetHorizontalCells(verticalId).ToList();
                Assert.AreEqual(h_size, horizontalCells.Count);
                for (int x = 0; x < h_size; x++)
                {
                    Assert.IsTrue(horizontalCells.Where(cell=>cell.HorizontalId == x && cell.VerticalId == verticalId).Count() == 1);
                }   
            }
        }
        
        [Test]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        public void Get_Vertical_Cells_Test(int h_size, int v_size)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(SimpleFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<GuardMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            protectMatrix.Log();
            for (int horizontalId = 0; horizontalId < h_size; horizontalId++)
            {
                var verticalCells = protectMatrix.GetVerticalCells(horizontalId).ToList();
                Assert.AreEqual(v_size, verticalCells.Count);
                for (int verticalId = 0; verticalId < v_size; verticalId++)
                {
                    Assert.IsTrue(verticalCells.Where(cell=>cell.HorizontalId == horizontalId && cell.VerticalId == verticalId).Count() == 1);
                }   
            }
        }
        [Test]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        public void Get_Cell_Test(int h_size, int v_size)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(RepeatFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<GuardMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            protectMatrix.Log();
            for (int verticalId = 0; verticalId < v_size; verticalId++)
            for (int horizontalId = 0; horizontalId < h_size; horizontalId++)
            {
                var cell = protectMatrix.GetCell(horizontalId, verticalId);
                Assert.AreEqual(horizontalId, cell.HorizontalId);
                Assert.AreEqual(verticalId, cell.VerticalId);
                if (cell is ValueCell valueCell)
                {
                    Assert.IsTrue(valueCell != null);
                    Assert.AreEqual((CellValueType)Mathf.RoundToInt(Mathf.Repeat(verticalId * h_size + horizontalId,
                            Enum.GetNames(typeof(CellValueType)).Length)), valueCell.Value);
                }
            }
        }
        
        [Test]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        public void Replace_Cell_Test(int h_size, int v_size)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(SimpleFillCellsFunc);
                mockCellProvider.SetMockFunc(() =>Container.Instantiate<ValueCell>( new object[]{ CellValueType.Eight }));
            }
            
            var protectMatrix = Container.Resolve<GuardMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            protectMatrix.Log();
            foreach (var cell in protectMatrix.GetCells())
                Assert.IsTrue(cell is ValueCell valueCell && valueCell.Value == CellValueType.Two);
            
            for (int verticalId = 0; verticalId < v_size; verticalId++)
            for (int horizontalId = 0; horizontalId < h_size; horizontalId++)
            {
                var cell = protectMatrix.ReplaceCell(horizontalId, verticalId);
                Debug.Log(string.Empty);
                protectMatrix.Log();
                Assert.IsTrue(cell is ValueCell valueCell && valueCell.Value == CellValueType.Eight);
            }
        }

        private IEnumerable<ICell> SimpleFillCellsFunc(int horiz, int vert)
        {
            var cells = new List<ICell>();
            for (var i = 0; i < horiz * vert; i++)
                cells.Add(Container.Instantiate<ValueCell>( new object[]{ CellValueType.Two }));
            return cells;
        }

        private IEnumerable<ICell> RepeatFillCellsFunc(int horiz, int vert)
        {
            var cells = new List<ICell>();
            for (var i = 0; i < horiz * vert; i++)
                cells.Add(Container.Instantiate<ValueCell>(new object[]
            {
                (CellValueType)Mathf.Repeat(i, Enum.GetNames(typeof(CellValueType)).Length)
            }));
            return cells;
        }


        private void AssertMatrixValid(GuardMatrix guardMatrix, int expectedHorizontalSize, int expectedVerticalSize)
        {
            Assert.IsTrue(guardMatrix.IsInitialized);
            var cells = guardMatrix.GetCells().ToList();
            Assert.AreEqual(expectedHorizontalSize, guardMatrix.Size.x);
            Assert.AreEqual(expectedVerticalSize, guardMatrix.Size.y);
            
            Assert.AreEqual(expectedHorizontalSize * expectedVerticalSize, cells.Count);
            for (int y = 0; y < expectedVerticalSize; y++)
            for (int x = 0; x < expectedHorizontalSize; x++)
            {
                var foundCells = cells.Where(cell => cell.HorizontalId == x && cell.VerticalId == y)
                    .ToList();
                Assert.AreEqual(1, foundCells.Count);
            }
        }
        
    }
}