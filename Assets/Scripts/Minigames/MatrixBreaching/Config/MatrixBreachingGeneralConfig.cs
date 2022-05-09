using UnityEngine;

namespace Minigames.MatrixBreaching.Config
{
    [CreateAssetMenu(fileName = "MatrixBreachingGeneralConfig", menuName = "Configs/Matrix Breaching/General",
        order = 0)]
    public class MatrixBreachingGeneralConfig : ScriptableObject
    {
        [SerializeField]
        private float _damageToVirusInterval;

        public float DamageToVirusInterval => _damageToVirusInterval;
    }
}