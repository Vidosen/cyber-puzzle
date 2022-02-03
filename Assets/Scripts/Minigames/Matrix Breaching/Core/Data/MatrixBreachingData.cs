using UnityEngine;

namespace Minigames.MatrixBreaching.Core.Data
{
    public static class MatrixBreachingData
    {
        public struct Settings
        {
            public Settings(string id, float damageToVirusPerSec, long targetProgress, Vector2Int matrixSize, int maxConcurrentVulnerabilities)
            {
                DamageToVirusPerSec = damageToVirusPerSec;
                TargetProgress = targetProgress;
                MatrixSize = matrixSize;
                MaxConcurrentVulnerabilities = maxConcurrentVulnerabilities;
                Id = id;
            }
            public string Id { get; }
            public float DamageToVirusPerSec { get;  }
            public long TargetProgress { get;  }
            public Vector2Int MatrixSize { get; }
            public int MaxConcurrentVulnerabilities { get; }
        }

        public struct MinigameResult
        {
            public MinigameResult(float breachingProgress, bool succeed)
            {
                BreachingProgress = breachingProgress;
                Succeed = succeed;
            }

            public float BreachingProgress { get; }
            public bool Succeed { get; }
            
        }

        public enum Status
        {
            Await,
            Process,
            Pause
        }
    }
}