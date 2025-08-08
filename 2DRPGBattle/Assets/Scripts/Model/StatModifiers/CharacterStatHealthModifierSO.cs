using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Player;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        if (BattleHandler.Instance != null && BattleHandler.Instance.IsPlayerTurn())
        {
            // In battle
            PlayerController pc = character.GetComponent<PlayerController>();
            if (pc != null)
                pc.Heal((int)val);
        }
        else
        {
            // In overworld
            PlayerOverworldController controller = character.GetComponent<PlayerOverworldController>();
            if (controller != null)
            {
                controller.HealPlayer((int)val);
            }
            else
            {
                Debug.LogError("No PlayerOverworldController found on: " + character.name);
            }
        }
    }
}