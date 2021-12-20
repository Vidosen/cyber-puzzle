using System;
using System.Collections.Generic;
using System.Linq;
using Minigames.MatrixBreaching.Matrix.Commands;
using Minigames.MatrixBreaching.Matrix.Data;
using Minigames.MatrixBreaching.Matrix.Interfaces;
using Minigames.MatrixBreaching.Matrix.Models;
using Minigames.MatrixBreaching.Matrix.Providers;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace Editor.Tests
{
    [TestFixture]
    public class MatrixOperationsTests : ZenjectUnitTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            Container.Bind<ProtectMatrix>().ToSelf().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MockCellProvider>().AsSingle().NonLazy();
        }



        [Test]
        [TestCase(3, 3, 1, 2)]
        [TestCase(3, 4, 0, 1)]
        [TestCase(4, 3, 0, 2)]
        [TestCase(4, 4, 0, 3)]
        [TestCase(5, 5, 0, 3)]
        public void Horizontal_Swap_Test(int h_size, int v_size, int firstRow, int secondRow)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(HorizontalFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<ProtectMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            LogMatrix(protectMatrix);
            AssertMatrixValid(protectMatrix, h_size, v_size);
            var horizontalSwapCommand = new HorizontalRowsSwapCommand(protectMatrix, firstRow, secondRow);
            horizontalSwapCommand.Execute();
            LogMatrix(protectMatrix);
            AssertMatrixValid(protectMatrix, h_size, v_size);
        }
        [Test]
        [TestCase(3, 3, 2, 20)]
        [TestCase(3, 4, 3, 2)]
        [TestCase(4, 3, 2, 2)]
        [TestCase(4, 4, 3, 2)]
        [TestCase(5, 5, 4, 2)]
        public void Horizontal_Scroll_Test(int h_size, int v_size, int rowId, int delta)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(VerticalFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<ProtectMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            LogMatrix(protectMatrix);
            AssertMatrixValid(protectMatrix, h_size, v_size);
            var horizontalSwapCommand = new HorizontalRowScrollCommand(protectMatrix, rowId, delta);
            horizontalSwapCommand.Execute();
            LogMatrix(protectMatrix);
            AssertMatrixValid(protectMatrix, h_size, v_size);
        }
        
        [Test]
        [TestCase(3, 3, 1, 2)]
        [TestCase(3, 4, 0, 1)]
        [TestCase(4, 3, 0, 1)]
        [TestCase(4, 4, 0, 1)]
        [TestCase(5, 5, 0, 1)]
        public void Vertical_Swap_Test(int h_size, int v_size, int firstRow, int secondRow)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(VerticalFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<ProtectMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            LogMatrix(protectMatrix);
            AssertMatrixValid(protectMatrix, h_size, v_size);
            var horizontalSwapCommand = new VerticalRowsSwapCommand(protectMatrix, firstRow, secondRow);
            horizontalSwapCommand.Execute();
            LogMatrix(protectMatrix);
            AssertMatrixValid(protectMatrix, h_size, v_size);
            
        }
        [Test]
        [TestCase(3, 3, 2, 20)]
        [TestCase(3, 4, 2, 2)]
        [TestCase(4, 3, 3, 2)]
        [TestCase(4, 4, 3, 2)]
        [TestCase(5, 5, 4, 2)]
        public void Vertical_Scroll_Test(int h_size, int v_size, int rowId, int delta)
        {
            {
                var mockCellProvider = Container.Resolve<MockCellProvider>();
                mockCellProvider.SetMockFunc(HorizontalFillCellsFunc);
            }
            
            var protectMatrix = Container.Resolve<ProtectMatrix>();
            protectMatrix.Initialize(h_size, v_size);
            LogMatrix(protectMatrix);
            AssertMatrixValid(protectMatrix, h_size, v_size);
            var horizontalSwapCommand = new VerticalRowScrollCommand(protectMatrix, rowId, delta);
            horizontalSwapCommand.Execute();
            LogMatrix(protectMatrix);
            AssertMatrixValid(protectMatrix, h_size, v_size);
        }

        private IEnumerable<ICell> HorizontalFillCellsFunc(int horiz, int vert)
        {
            var cells = new List<ICell>();
            for (var y = 0; y < vert; y++)
            for (var x = 0; x < horiz; x++)
                cells.Add(new ValueCell((CellValue)Mathf.Repeat(y, Enum.GetNames(typeof(CellValue)).Length)));
            return cells;
        }
        private IEnumerable<ICell> VerticalFillCellsFunc(int horiz, int vert)
        {
            var cells = new List<ICell>();
            for (var y = 0; y < vert; y++)
            for (var x = 0; x < horiz; x++)
                cells.Add(new ValueCell((CellValue)Mathf.Repeat(x, Enum.GetNames(typeof(CellValue)).Length)));
            return cells;
        }


        private void AssertMatrixValid(ProtectMatrix protectMatrix, int expectedHorizontalSize, int expectedVerticalSize)
        {
            Assert.IsTrue(protectMatrix.IsInitialized);
            var cells = protectMatrix.GetCells().ToList();
            Assert.AreEqual(expectedHorizontalSize, protectMatrix.Size.x);
            Assert.AreEqual(expectedVerticalSize, protectMatrix.Size.y);
            
            Assert.AreEqual(expectedHorizontalSize * expectedVerticalSize, cells.Count);
            for (int y = 0; y < expectedVerticalSize; y++)
            for (int x = 0; x < expectedHorizontalSize; x++)
            {
                var foundCells = cells.Where(cell => cell.HorizontalId == x && cell.VerticalId == y)
                    .ToList();
                Assert.AreEqual(1, foundCells.Count);
            }
        }
        
        private void LogMatrix(ProtectMatrix protectMatrix)
        {
            Debug.Log("== Matrix :: Start ==");
            for (int y = 0; y < protectMatrix.Size.y; y++)
            {
                var row = protectMatrix.GetHorizontalCells(y);
                var rowLogLine = row.Select(cell => cell as ValueCell).Where(cell => cell != null)
                    .Select(cell => (int)cell.Value + "\t").Aggregate((one, two) => one + two);
                Debug.Log(rowLogLine);
            }
            Debug.Log("== Matrix :: End ==");
        }
    }
}