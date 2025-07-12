using UnityEngine;

namespace Enemy.Model
{
    [CreateAssetMenu]
    public class EnemySO : ScriptableObject
    {
        [Header("Basic Info")]
        public string enemyName;
        public Sprite enemySprite;
        [TextArea] public string enemyDescription;
        public EnemyType enemyType;

        [Header("Health Stats")]
        public int baseHealth = 100;

        [Header("Attack 1 Stats")]
        public int minDmg = 7;
        public int maxDmg = 12;
        public int critChance = 5;

        [Header("Attack 2 Stats")]
        public int minDmg2 = 8;
        public int maxDmg2 = 13;
        public int critChance2 = 15;

        [Header("Experience Reward")]
        public int expReward = 50;
        public int goldReward = 100;

        [Header("Visuals")]
        public RuntimeAnimatorController animatorController;

        public enum EnemyType { Melee, Ranged, Boss }
    }
}