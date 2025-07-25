using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        PlayerOverworldController controller = character.GetComponent<PlayerOverworldController>();
        if (controller != null)
        {
            controller.HealPlayer((int)val);
            Debug.Log($"Healed player by {val} points.");
        }
        else
        {
            Debug.LogWarning("No PlayerOverworldController found on character.");
        }
    }
}