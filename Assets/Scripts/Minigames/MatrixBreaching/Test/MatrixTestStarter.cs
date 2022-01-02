using System;
using Minigames.MatrixBreaching.Matrix.Models;
using UnityEngine;
using Utils;
using Zenject;

namespace Minigames.MatrixBreaching.Test
{
    public class MatrixTestStarter : MonoBehaviour
    {
        [Min(2)] public int HorizontalSize;
        [Min(2)] public int VerticalSize;
        private GuardMatrix _guardMatrix;
        

        [Inject]
        private void Construct(GuardMatrix guardMatrix)
        {
            _guardMatrix = guardMatrix;
        }

        private void Start()
        {
            _guardMatrix.Initialize(HorizontalSize, VerticalSize);
            _guardMatrix.Log();
        }
    }
}