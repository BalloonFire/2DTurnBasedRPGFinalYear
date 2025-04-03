using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightMana : MonoBehaviour
{
    public float maxMana = 100f;
    public float currentMana { get; private set; }

    void Start() => currentMana = maxMana;

    public bool CanUseAbility(float cost) => currentMana >= cost;

    public void UseMana(float cost)
    {
        currentMana = Mathf.Clamp(currentMana - cost, 0f, maxMana);
    }
}