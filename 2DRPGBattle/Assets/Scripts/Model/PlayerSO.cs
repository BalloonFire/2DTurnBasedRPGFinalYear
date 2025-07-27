using UnityEngine;
using Player.Model;

namespace Player.Model
{
    [CreateAssetMenu]
    public class PlayerSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string className;
        public Sprite classIcon;
        [TextArea] public string classDescription;

        [Header("Health Settings")]
        public int baseHealth = 100;

        [Header("Basic Attack")]
        public int minDmgAtk = 10;
        public int maxDmgAtk = 15;
        public int critChanceAtk = 15;

        [Header("Skill Attack")]
        public int minDmgSkill = 20;
        public int maxDmgSkill = 25;
        public int critChanceSkill = 25;
        public int skillManaCost = 1;

        [Header("Ultimate Attack")]
        public int minDmgUltimate = 35;
        public int maxDmgUltimate = 45;
        public int critChanceUltimate = 50;
        public int ultimateManaCost = 5;

        [Header("Visuals")]
        public RuntimeAnimatorController overworldAnimatorController;
        public RuntimeAnimatorController battleAnimatorController;
    }
}