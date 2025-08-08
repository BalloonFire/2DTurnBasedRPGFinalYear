using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EdibleSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData> modifiersData = new List<ModifierData>();
        public string ActionName => "Consume";

        [SerializeField] private AudioClip actionSFX;
        AudioClip IItemAction.actionSFX => actionSFX;

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            if (BattleHandler.Instance != null && BattleHandler.Instance.IsPlayerTurn())
            {
                // Heal all alive players in battle
                var allPlayers = UnityEngine.Object.FindObjectsOfType<PlayerController>();
                foreach (var player in allPlayers)
                {
                    if (player.IsAlive())
                    {
                        foreach (ModifierData data in modifiersData)
                        {
                            data.statModifier.AffectCharacter(player.gameObject, data.value);
                        }
                    }
                }
            }
            else
            {
                // Overworld - only the one character
                foreach (ModifierData data in modifiersData)
                {
                    Debug.Log("Edible item used on: " + character.name);
                    data.statModifier.AffectCharacter(character, data.value);
                }
            }

            return true;
        }

    }

    public interface IDestroyableItem
    {
    }

    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
        bool PerformAction(GameObject character, List<ItemParameter> itemState);
    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifier;
        public float value;
    }
}
