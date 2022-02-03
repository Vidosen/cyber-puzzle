using System;
using Minigames.MatrixBreaching.Core.Data;
using Minigames.MatrixBreaching.Core.Services;
using Minigames.MatrixBreaching.Matrix.Models;
using UnityEngine;
using Viruses.Models;
using Zenject;

namespace Minigames.MatrixBreaching.Test
{
    public class MatrixTestStarter : MonoBehaviour
    {
        [Min(2)] public int HorizontalSize;
        [Min(2)] public int VerticalSize;
        [Min(2)] public int TargetProgress = 500;
        [Min(2)] public float DamagePerSec = 10;
        [Min(2)] public int VulnerabilitiesCount = 3;
        
        private MatrixBreachingService _breachingService;


        [Inject]
        private void Construct(GuardMatrix guardMatrix, MatrixBreachingService breachingService)
        {
            _breachingService = breachingService;
        }

        private async void Start()
        {
            Debug.Log($"Started Minigame");
            var result = await _breachingService.StartMinigame(new VirusModel().Initialize("", 10f, 300, 0),
                new MatrixBreachingData.Settings("test",
                    DamagePerSec, TargetProgress, new Vector2Int(HorizontalSize, VerticalSize),
                    VulnerabilitiesCount));
            Debug.Log($"Finished Minigame: Success::{result.Succeed}, Progress::{result.BreachingProgress}");
        }
    }
}