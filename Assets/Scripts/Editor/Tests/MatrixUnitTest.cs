using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MatrixUnitTest : ZenjectUnitTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            Container.Bind<ProtectMatrix>().ToSelf().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MockCellProvider>().AsSingle().NonLazy();
        }

        [Test]
        [TestCase(3,3)]
        [TestCase(3,4)]
        [TestCase(4,3)]
        [TestCase(4,4)]
        [TestCase(5,5)]
        public void IsNewlyCreatedMatrixValid(int h_Size, int v_size)
        {
            var mockCellProvider = Container.Resolve<MockCellProvider>();
            var protectMatrix = Container.Resolve<ProtectMatrix>();
            Func<int, IEnumerable<ICell>> simpleFillCellsFunc = (size) =>
            {
                List<ICell> cells = new List<ICell>();
                for (int i = 0; i < size; i++)
                    cells.Add(new ValueCell(CellValue.Zero));
                return cells;
            };
            mockCellProvider.SetMockFunc(simpleFillCellsFunc);
            protectMatrix.Initialize(h_Size, v_size);
            AssertMatrixValid(protectMatrix, h_Size, v_size);
            LogMatrix(protectMatrix);
        }

        private void AssertMatrixValid(ProtectMatrix protectMatrix, int expectedHorizontalSize, int expectedVerticalSize)
        {
            Assert.IsTrue(protectMatrix.IsInitialized);
            var cells = protectMatrix.GetCells().ToList();
            Assert.AreEqual(expectedHorizontalSize, protectMatrix.Size.x);
            Assert.AreEqual(expectedVerticalSize, protectMatrix.Size.y);
            
            Assert.AreEqual(expectedHorizontalSize * expectedVerticalSize, cells.Count);
            for (int x = 0; x < expectedHorizontalSize; x++)
            {
                for (int y = 0; y < expectedVerticalSize; y++)
                {
                    var foundCells = cells.Where(cell => cell.HorizontalId == x && cell.VerticalId == y)
                        .ToList();
                    Assert.AreEqual(1, foundCells.Count);
                }
            }
        }
        
        private void LogMatrix(ProtectMatrix protectMatrix)
        {
            Debug.Log("== Matrix :: Start ==");
            for (int y = 0; y < protectMatrix.Size.y; y++)
            {
                var row = protectMatrix.GetVerticalCells(y).ToList();
                var rowLogLine = row.Select(cell => cell as ValueCell).Where(cell => cell != null)
                    .Select(cell => cell.Value + "\t").Aggregate((one, two) => one + two);
                Debug.Log(rowLogLine);
            }
            Debug.Log("== Matrix :: End ==");
        }
    }
}