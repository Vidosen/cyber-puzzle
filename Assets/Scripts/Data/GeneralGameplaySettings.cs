using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GeneralGameplaySettings", menuName = "Settings/General Gameplay Settings")]
    public class GeneralGameplaySettings : ScriptableObject, ICombinationSettings, ILevelSettings
    {
        [Tooltip("Hack Points required to win the level")]
        [SerializeField] protected double _hpRequired;
        [Tooltip("Hack Points required to win the level")]
        [SerializeField] protected double _levelTimerDuration;
        
        [Header("Combination Settings")]
        [Tooltip("Hack Points added to the combination per one cell")]
        [SerializeField] protected double _hpRewardForCell;
        [Tooltip("Hack Points added to the combination for a disjoint pair of cells")]
        [SerializeField] protected double _hpRewardForDisjointPair;
        [Tooltip("Hack Points added to the combination for a not neighboring pair of cells")]
        [SerializeField] protected double _hpRewardForNotNeighboringPair;


        public double HPRequired => _hpRequired;
        public double LevelTimerDuration => _levelTimerDuration;

        public double HPRewardForCell => _hpRewardForCell;
        public double HPRewardForDisjointPair => _hpRewardForDisjointPair;
        public double HPRewardForNotNeighboringPair => _hpRewardForNotNeighboringPair;
    }
}
